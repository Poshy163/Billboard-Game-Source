#region

using Newtonsoft.Json;
using Other;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming

#endregion

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
// ReSharper disable StringLiteralTypo
// ReSharper disable ParameterHidesMember
// ReSharper disable PossibleNullReferenceException
namespace UI
{
    public class MainMenu:MonoBehaviour
    {
        private static bool _loginPage = true;
        public TMP_Text debugtxt;
        public TMP_Text version;
        public GameObject TutorialBox;
        private Toggle _toggle;
        private bool _tutorial;

        private void Start ()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            version.text = "";
            _toggle = GameObject.Find("Leaderboard Toggle").GetComponent<Toggle>();
            _toggle.isOn = DisplayHighscores.LoadHighScores;
            GetGameVersion();
        }


        private void Update ()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if(Input.GetKeyDown(KeyCode.Return) && _loginPage)
            {
                Login();
            }
            else if(Input.GetKeyDown(KeyCode.Return) && !_loginPage)
            {
                ShowTutorialBox();
            }
        }

        private bool CheckServerStatus ()
        {
            try
            {
                if(Database.GetServerStatus())
                {
                    return true;
                }

                debugtxt.text = "Server is down, Please wait a while before trying again. Your details have been saved";
                Invoke(nameof(StartCoolDown),5f);
                return false;
            }
            catch
            {
                debugtxt.text = "Cannot connect to the server, Please check your connection";
                Invoke(nameof(StartCoolDown),5f);
                return false;
            }
        }


        public void ShowTutorialBox ()
        {
            TutorialBox.SetActive(true);
        }


        public void InvertLogin ()
        {
            _loginPage = !_loginPage;
        }

        public void Login ()
        {
            debugtxt.text = "Loading...";
            var localname = GameObject.Find("NameLogin").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordLogin").GetComponent<TMP_InputField>().text.Trim();
            if(string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
            {
                debugtxt.text = "One or more fields cannot be empty";
                Invoke(nameof(StartCoolDown),2.5f);
                return;
            }

            if(!CheckServerStatus())
            {
                return;
            }

            if(Database.Login(localname,password))
            {
                GlobalVar.Name = localname;
                SendSlackMessage($"User logged in with the username: {localname}");
                debugtxt.text = "Login Completed, Welcome!";
                SetOtherStuff(localname);
                SceneManager.LoadScene("Lobby");
            }
            else
            {
                debugtxt.text = "Either this name doesnt exist or you have the wrong password";
                Invoke(nameof(StartCoolDown),2.5f);
            }
        }

        public void StartCoolDown ()
        {
            debugtxt.text = "";
        }


        public void Yes ()
        {
            _tutorial = true;
            SignUp();
        }

        public void No ()
        {
            _tutorial = false;
            SignUp();
        }


        private async void SignUp ()
        {
            debugtxt.text = "Loading...";
            var localname = GameObject.Find("NameSignUp").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordSignUp").GetComponent<TMP_InputField>().text.Trim();

            if(!CheckServerStatus())
            {
                return;
            }

            if(await CheckForBadWords(localname))
            {
                TutorialBox.SetActive(false);
                debugtxt.text = "Nice try, dont have bad words in your name";
                Invoke(nameof(StartCoolDown),2.5f);
                return;
            }

            if(string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
            {
                TutorialBox.SetActive(false);
                debugtxt.text = "One or more fields cannot be empty";
                Invoke(nameof(StartCoolDown),2.5f);
                return;
            }

            if(localname.Length >= 15)
            {
                TutorialBox.SetActive(false);
                debugtxt.text = "Username is too long, max is 15 characters";
                Invoke(nameof(StartCoolDown),2.5f);
                return;
            }

            if(Database.SignUp(localname,password))
            {
                GlobalVar.Name = localname;
                SendSlackMessage($"User signed up with the username: {localname}");
                SetOtherStuff(localname);
                GlobalVar.IsSignUp = true;
                debugtxt.text = "Completed Sign up";
                SceneManager.LoadScene(_tutorial ? "Tutorial" : "Lobby");
            }
            else
            {
                TutorialBox.SetActive(false);
                debugtxt.text = "This username is already in use";
                Invoke(nameof(StartCoolDown),2.5f);
            }
        }

        private static void SetOtherStuff ( string name )
        {
            switch(GameObject.Find("Difficulty").GetComponent<TMP_Dropdown>().value)
            {
                case 0:
                    GlobalVar.GameDifficulty = GlobalVar.GameDifficultyEnum.Easy;
                    break;
                case 1:
                    GlobalVar.GameDifficulty = GlobalVar.GameDifficultyEnum.Normal;
                    break;
                case 2:
                    GlobalVar.GameDifficulty = GlobalVar.GameDifficultyEnum.Hard;
                    break;
            }

            if(!Application.isEditor)
            {
                Database.GetUserInfo(name);
            }

            GlobalVar.UpdateSettings();
        }

        private static async Task<bool> CheckForBadWords ( string name )
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://neutrinoapi-bad-word-filter.p.rapidapi.com/bad-word-filter"),
                    Headers =
                    {
                        { "x-rapidapi-key", "1ebdde9fcamsh8ba5f97b8811643p177dfajsn16cfa7d49c86" },
                        { "x-rapidapi-host", "neutrinoapi-bad-word-filter.p.rapidapi.com" }
                    },
                    Content = new FormUrlEncodedContent(new Dictionary<string,string>
                    {
                        { "censor-character", "*" },
                        { "content", name }
                    })
                };
                var response = await client.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();
                dynamic jsonFile = JsonConvert.DeserializeObject(body);
                return bool.Parse(jsonFile["is-bad"].ToString());
            }
            catch
            {
                return false;
            }
        }

        public static void PermDeleteAccount ()
        {
            for(short i = 0;i <= 25;i++)
            {
                try
                {
                    Database.DeleteDatabaseEntry(GlobalVar.Name,i);
                }
                catch
                {
                    // ignored
                }
            }

            Database.DeleteUser(GlobalVar.Name);
            SceneManager.LoadScene("Main Menu");
        }

        public void LoadLeaderboardToggle ()
        {
            _toggle.isOn = !DisplayHighscores.LoadHighScores;
            DisplayHighscores.LoadHighScores = _toggle.isOn;
        }

        private void GetGameVersion ()
        {
            var path = Path.Combine(@"C:\Users\" + Environment.UserName + @"\Videos\Game","MonoBleedingEdge",
                "Version.txt");
            var altar = Path.Combine(Directory.GetCurrentDirectory(),"MonoBleedingEdge","Version.txt");
            if(File.Exists(path))
            {
                version.text = File.ReadAllText(path);
            }
            else if(File.Exists(altar))
            {
                version.text = File.ReadAllText(altar);
            }
            else
            {
                version.text = "1.9";
            }
        }

        private static void SendSlackMessage ( string message )
        {
            try
            {
                var client =
                    new SlackClient(
                        "https://hooks.slack.com/services/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");
                client.PostMessage(username: "User Login",text: message,channel: "#user-login");
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    public class SlackClient
    {
        private readonly Encoding _encoding = new UTF8Encoding();
        private readonly Uri _uri;

        public SlackClient ( string urlWithAccessToken )
        {
            _uri = new Uri(urlWithAccessToken);
        }

        public void PostMessage ( string text,string username = null,string channel = null )
        {
            var payload = new Payload
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            PostMessage(payload);
        }

        private void PostMessage ( Payload payload )
        {
            var payloadJson = JsonConvert.SerializeObject(payload);

            using(var client = new WebClient())
            {
                var data = new NameValueCollection { ["payload"] = payloadJson };
                client.UploadValues(_uri,"POST",data);
            }
        }
    }

    public class Payload
    {
        [JsonProperty("channel")] public string Channel { get; set; }

        [JsonProperty("username")] public string Username { get; set; }

        [JsonProperty("text")] public string Text { get; set; }
    }
}