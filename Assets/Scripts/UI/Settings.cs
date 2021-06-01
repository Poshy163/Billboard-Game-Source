using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ReSharper disable StringLiteralTypo
// ReSharper disable ParameterHidesMember

// ReSharper disable PossibleNullReferenceException

namespace UI
{
    public class Settings : MonoBehaviour
    {
        public TMP_Text debugtxt;
        public TMP_Text version;
        private Toggle _toggle;

        private void Start()
        {
            _toggle = GameObject.Find("Leaderboard Toggle").GetComponent<Toggle>();
            _toggle.isOn = DisplayHighscores.LoadHighScores;
            GetGameVersion();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }

        public void Login()
        {
            var localname = GameObject.Find("NameLogin").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordLogin").GetComponent<TMP_InputField>().text.Trim();

            if (string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
            {
                debugtxt.text = "One or more fields cannot be empty";
                return;
            }

            if (Saving.Saving.Login(localname, password))
            {
                GlobalVar.Name = localname;
                debugtxt.text = "Login Completed, Welcome!";
                SceneManager.LoadScene("LevelSelect");
            }
            else
            {
                debugtxt.text = "Either this _name doesnt exist or you have the wrong password";
            }
        }

        public async void SignUp()
        {
            var localname = GameObject.Find("NameSignUp").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordSignUp").GetComponent<TMP_InputField>().text.Trim();
            if (await CheckForBadWords(localname))
            {
                debugtxt.text = "Nice try, dont do bad words";
                return;
            }

            if (string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
            {
                debugtxt.text = "One or more fields cannot be empty";
                return;
            }


            if (localname.Length >= 15)
            {
                debugtxt.text = "Username is too long, max is 15 characters";
                return;
            }

            if (Saving.Saving.SignUp(localname, password))
            {
                GlobalVar.Name = localname;
                debugtxt.text = "Done Sign up";
                SceneManager.LoadScene("LevelSelect");
            }
            else
            {
                debugtxt.text = "This _name is already in use";
            }
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
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Version.txt");
            if (File.Exists(path))
                version.text = File.ReadAllText(path);
            else
                version.gameObject.SetActive(false);
        }
    }
}