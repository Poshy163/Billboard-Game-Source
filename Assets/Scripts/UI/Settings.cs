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
// ReSharper disable StringLiteralTypo
// ReSharper disable ParameterHidesMember
// ReSharper disable PossibleNullReferenceException
namespace UI
{
    public class Settings:MonoBehaviour
    {
        public TMP_Text debugtxt;
        public TMP_Text version;
        private Toggle _toggle;
        public static bool loginPage = true;

        private void Start ()
        {
            _toggle = GameObject.Find("Leaderboard Toggle").GetComponent<Toggle>();
            _toggle.isOn = DisplayHighscores.LoadHighScores;
            GetGameVersion();
        }

        public void InvertLogin ()
        {
            loginPage = !loginPage;
        }

        private void Update ()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if(Input.GetKeyDown(KeyCode.Return) && loginPage)
            {
                Login();
            }
            else if(Input.GetKeyDown(KeyCode.Return) && !loginPage)
            {
                SignUp();
            }
        }

        public void Login ()
        {
            debugtxt.text = "Loading...";
            string localname = GameObject.Find("NameLogin").GetComponent<TMP_InputField>().text.Trim();
            string password = GameObject.Find("PasswordLogin").GetComponent<TMP_InputField>().text.Trim();
            if(string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
            {
                debugtxt.text = "One or more fields cannot be empty";
                Invoke("StartCoolDown",2.5f);
                return;
            }

            if(Saving.Saving.Login(localname,password))
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
                Invoke("StartCoolDown",2.5f);
            }
        }

        public void StartCoolDown ()
        {
            debugtxt.text = "";
        }



        public async void SignUp ()
        {
            debugtxt.text = "Loading...";
            string localname = GameObject.Find("NameSignUp").GetComponent<TMP_InputField>().text.Trim();
            string password = GameObject.Find("PasswordSignUp").GetComponent<TMP_InputField>().text.Trim();
            if(await CheckForBadWords(localname))
            {
                debugtxt.text = "Nice try, dont do bad words";
                Invoke("StartCoolDown",2.5f);
                return;
            }

            if(string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
            {
                debugtxt.text = "One or more fields cannot be empty";
                Invoke("StartCoolDown",2.5f);
                return;
            }

            if(localname.Length >= 15)
            {
                debugtxt.text = "Username is too long, max is 15 characters";
                Invoke("StartCoolDown",2.5f);
                return;
            }

            if(Saving.Saving.SignUp(localname,password))
            {
                GlobalVar.Name = localname;
                SendSlackMessage($"User signed up with the username: {localname}");
                SetOtherStuff();
                debugtxt.text = "Done Sign up";
                SceneManager.LoadScene("LevelSelect");
            }
            else
            {
                debugtxt.text = "This username is already in use";
                Invoke("StartCoolDown",2.5f);
                return;
            }
        }

        private static void SetOtherStuff ()
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

            GlobalVar.UpdateSettings();
        }

        private static async Task<bool> CheckForBadWords ( string name )
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://neutrinoapi-bad-word-filter.p.rapidapi.com/bad-word-filter"),
                Headers =
                {
                    {"x-rapidapi-key", "1ebdde9fcamsh8ba5f97b8811643p177dfajsn16cfa7d49c86"},
                    {"x-rapidapi-host", "neutrinoapi-bad-word-filter.p.rapidapi.com"}
                },
                Content = new FormUrlEncodedContent(new Dictionary<string,string>
                {
                    {"censor-character", "*"},
                    {"content", name}
                })
            };
            HttpResponseMessage response = await client.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();
            dynamic jsonFile = JsonConvert.DeserializeObject(body);
            return bool.Parse(jsonFile["is-bad"].ToString());
        }

        public static void PermDeleteAccount ()
        {
            for(short i = 0;i <= 25;i++)
            {
                try
                {
                    Saving.Saving.DeleteDatabaseEntry(GlobalVar.Name,i);
                }
                catch
                {
                    // ignored
                }
            }

            Saving.Saving.DeleteUser(GlobalVar.Name);
            SceneManager.LoadScene("Settings");
        }

        public void LoadLeaderboardToggle ()
        {
            _toggle.isOn = !DisplayHighscores.LoadHighScores;
            DisplayHighscores.LoadHighScores = _toggle.isOn;
        }

        private void GetGameVersion ()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(),"Version.txt");
            if(File.Exists(path))
            {
                version.text = File.ReadAllText(path);
            }
            else
            {
                version.gameObject.SetActive(false);
            }
        }

        private static void SendSlackMessage ( string message )
        {
            try
            {
                SlackClient client =
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
            Payload payload = new Payload
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            PostMessage(payload);
        }

        private void PostMessage ( Payload payload )
        {
            string payloadJson = JsonConvert.SerializeObject(payload);

            using(WebClient client = new WebClient())
            {
                NameValueCollection data = new NameValueCollection { ["payload"] = payloadJson };
                byte[] response = client.UploadValues(_uri,"POST",data);
                // ReSharper disable once UnusedVariable
                string responseText = _encoding.GetString(response);
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