using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Windows;

namespace GameLauncher
{
    internal enum LauncherStatus
    {
        ready,
        failed,
        downloadingGame,
        downloadingUpdate
    }

    public partial class MainWindow : Window
    {
        private readonly string rootPath;
        private readonly string gameZip;
        private string gameExe;
        private readonly string ZipName = "NewVersion";

        private LauncherStatus _status;

        internal LauncherStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (_status)
                {
                    case LauncherStatus.ready:
                        PlayButton.Content = "Play";
                        break;

                    case LauncherStatus.failed:
                        PlayButton.Content = "Update Failed - Retry";
                        break;

                    case LauncherStatus.downloadingGame:
                        PlayButton.Content = "Downloading Game";
                        break;

                    case LauncherStatus.downloadingUpdate:
                        PlayButton.Content = "Downloading Update";
                        break;

                    default:
                        break;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            rootPath = Directory.GetCurrentDirectory();
            gameZip = Path.Combine(rootPath, ZipName + ".zip");
        }

        private void UpdateFilePaths(string local)
        {
            gameExe = Path.Combine(rootPath, "Poshy163-Billboard-Game-" + local, "Billboard Shooter.exe");
        }

        private void CheckForUpdates()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            var json = client.GetAsync("https://api.github.com/repos/Poshy163/Billboard-Game/commits").Result.Content.ReadAsStringAsync().Result;
            dynamic commits = JArray.Parse(json);
            string lastCommit = commits[0].commit.message;
            string OnlineVerion = lastCommit.Split("\n")[0].Split(" ")[1];
            string LocalFileName = (commits[0].sha);
            string ShortFile = LocalFileName.Substring(0, 7);

            UpdateFilePaths(ShortFile);
            string localVersion = File.ReadAllText(Path.Combine(rootPath, "Poshy163-Billboard-Game-" + ShortFile, "Version.txt"));
            MessageBox.Show(localVersion);
            VersionText.Text = localVersion.ToString();

            try
            {
                if (localVersion != OnlineVerion)
                {
                    Status = LauncherStatus.downloadingGame;
                    InstallGameFiles(true);
                }
                else
                {
                    Status = LauncherStatus.ready;
                    MessageBox.Show("Up to date");
                    return;
                }
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error checking for game updates: {ex}");
            }
        }

        private void InstallGameFiles(bool _isUpdate)
        {
            try
            {
                WebClient webClient = new WebClient();
                if (_isUpdate)
                {
                    Status = LauncherStatus.ready;
                }
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.DownloadFileAsync(new Uri("https://api.github.com/repos/Poshy163/Billboard-Game/zipball"), ZipName + ".zip");
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error installing game files: {ex}");
            }
        }

        private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                ZipFile.ExtractToDirectory(gameZip, rootPath, true);
                File.Delete(gameZip);
                Status = LauncherStatus.ready;
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hit");
            if (File.Exists(gameExe) && Status == LauncherStatus.ready)
            {
                MessageBox.Show("Starting");
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                Process.Start(startInfo);
                Close();
            }
            else if (Status == LauncherStatus.failed)
            {
                MessageBox.Show("Launcher Failed, retrying");
                CheckForUpdates();
            }
            //Its skipping to here for no apparent reason
        }
    }
}