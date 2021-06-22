#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Other;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

// ReSharper disable StringLiteralTypo
// ReSharper disable ParameterHidesMember
// ReSharper disable PossibleNullReferenceException
namespace UI
{
    public class Settings : MonoBehaviour
    {
        public static bool loginPage = true;
        public TMP_Text debugtxt;
        public TMP_Text version;
        public GameObject TutorialBox;
        private Toggle _toggle;
        private bool tutorial;

        private void Start()
        {
            version.text = "";
            _toggle = GameObject.Find("Leaderboard Toggle").GetComponent<Toggle>();
            _toggle.isOn = DisplayHighscores.LoadHighScores;
            GetGameVersion();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

            if (Input.GetKeyDown(KeyCode.Return) && loginPage)
                Login();
            else if (Input.GetKeyDown(KeyCode.Return) && !loginPage) ShowTutorialBox();
        }


        public void ShowTutorialBox()
        {
            TutorialBox.SetActive(true);
        }


        public void InvertLogin()
        {
            loginPage = !loginPage;
        }

        public void Login()
        {
            debugtxt.text = "Loading...";
            var localname = GameObject.Find("NameLogin").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordLogin").GetComponent<TMP_InputField>().text.Trim();
            if (string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
            {
                debugtxt.text = "One or more fields cannot be empty";
                Invoke("StartCoolDown", 2.5f);
                return;
            }

            if (Saving.Saving.Login(localname, password))
            {
                GlobalVar.Name = localname;
                SendSlackMessage($"User logged in with the username: {localname}");
                debugtxt.text = "Login Completed, Welcome!";
                SetOtherStuff();
                SceneManager.LoadScene("LevelSelect");
            }
            else
            {
                debugtxt.text = "Either this name doesnt exist or you have the wrong password";
                Invoke(nameof(StartCoolDown), 2.5f);
            }
        }

        public void StartCoolDown()
        {
            debugtxt.text = "";
        }


        public void Yes()
        {
            tutorial = true;
            SignUp();
        }

        public void No()
        {
            tutorial = false;
            SignUp();
        }


        private async void SignUp()
        {
            debugtxt.text = "Loading...";
            var localname = GameObject.Find("NameSignUp").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordSignUp").GetComponent<TMP_InputField>().text.Trim();
            if (await CheckForBadWords(localname))
            {
                TutorialBox.SetActive(false);
                debugtxt.text = "Nice try, dont do bad words";
                Invoke("StartCoolDown", 2.5f);
                return;
            }

            if (string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
            {
                TutorialBox.SetActive(false);
                debugtxt.text = "One or more fields cannot be empty";
                Invoke("StartCoolDown", 2.5f);
                return;
            }

            if (localname.Length >= 15)
            {
                TutorialBox.SetActive(false);
                debugtxt.text = "Username is too long, max is 15 characters";
                Invoke("StartCoolDown", 2.5f);
                return;
            }

            if (Saving.Saving.SignUp(localname, password))
            {
                GlobalVar.Name = localname;
                SendSlackMessage($"User signed up with the username: {localname}");
                SetOtherStuff();
                GlobalVar.IsSignUp = true;
                debugtxt.text = "Done Sign up";
                if (tutorial)
                    SceneManager.LoadScene("Tutorial");
                else
                    SceneManager.LoadScene("LevelSelect");
            }
            else
            {
                TutorialBox.SetActive(false);
                debugtxt.text = "This username is already in use";
                Invoke("StartCoolDown", 2.5f);
            }
        }

        private static void SetOtherStuff()
        {
            switch (GameObject.Find("Difficulty").GetComponent<TMP_Dropdown>().value)
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

            GlobalVar.UpdateSettings();
        }

        private static async Task<bool> CheckForBadWords(string name)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://neutrinoapi-bad-word-filter.p.rapidapi.com/bad-word-filter"),
                Headers =
                {
                    {"x-rapidapi-key", "1ebdde9fcamsh8ba5f97b8811643p177dfajsn16cfa7d49c86"},
                    {"x-rapidapi-host", "neutrinoapi-bad-word-filter.p.rapidapi.com"}
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"censor-character", "*"},
                    {"content", name}
                })
            };
            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            dynamic jsonFile = JsonConvert.DeserializeObject(body);
            return bool.Parse(jsonFile["is-bad"].ToString());
        }

        public static void PermDeleteAccount()
        {
            for (short i = 0; i <= 25; i++)
                try
                {
                    Saving.Saving.DeleteDatabaseEntry(GlobalVar.Name, i);
                }
                catch
                {
                    // ignored
                }

            Saving.Saving.DeleteUser(GlobalVar.Name);
            SceneManager.LoadScene("Settings");
        }

        public void LoadLeaderboardToggle()
        {
            _toggle.isOn = !DisplayHighscores.LoadHighScores;
            DisplayHighscores.LoadHighScores = _toggle.isOn;
        }

        private void GetGameVersion()
        {
            var path = Path.Combine(@"C:\Users\" + Environment.UserName + @"\Videos\Game", "MonoBleedingEdge",
                "Version.txt");
            if (File.Exists(path))
                version.text = File.ReadAllText(path);
            else
                version.gameObject.SetActive(false);
        }

        private static void SendSlackMessage(string message)
        {
            try
            {
                var client =
                    new SlackClient(
                        "https://hooks.slack.com/services/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");
                client.PostMessage(username: "User Login", text: message, channel: "#user-login");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    public class SlackClient
    {
        private readonly Encoding _encoding = new UTF8Encoding();
        private readonly Uri _uri;

        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        public void PostMessage(string text, string username = null, string channel = null)
        {
            var payload = new Payload
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            PostMessage(payload);
        }

        private void PostMessage(Payload payload)
        {
            var payloadJson = JsonConvert.SerializeObject(payload);

            using (var client = new WebClient())
            {
                var data = new NameValueCollection {["payload"] = payloadJson};
                client.UploadValues(_uri, "POST", data);
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