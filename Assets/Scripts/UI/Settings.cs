using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
// ReSharper disable PossibleNullReferenceException

namespace UI
{
    public class Settings : MonoBehaviour
    {
        public TMP_Text debugtxt;
        private Toggle toggle;
        public TMP_Text Version;
        private void Start ()
        {
            toggle = GameObject.Find("Leaderboard Toggle").GetComponent<Toggle>();
            toggle.isOn = DisplayHighscores.LoadHighScores;
            GetGameVersion();
        }

        public void Login()
        {
            var localname = GameObject.Find("NameLogin").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordLogin").GetComponent<TMP_InputField>().text.Trim();

            if(string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
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
                debugtxt.text = "Either this name doesnt exist or you have the wrong password";
        }

        public void SignUp()
        {
            var localname = GameObject.Find("NameSignUp").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordSignUp").GetComponent<TMP_InputField>().text.Trim();

            if(string.IsNullOrEmpty(localname) || string.IsNullOrEmpty(password))
            {
                debugtxt.text = "One or more fields cannot be empty";
                return;
            }


            if(localname.Length >= 15)
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
                debugtxt.text = "This name is already in use";
        }

        public static void PermDeleteAccount()
        {
            for (short i = 0; i <= 25; i++)
            {
                try
                {
                    Saving.Saving.DeleteDatabaseEntry(GlobalVar.Name, i);
                }
                catch {}
            }

            Saving.Saving.DeleteUser(GlobalVar.Name);
            SceneManager.LoadScene("Settings");

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void LoadLeaderboardToggle()
        {
            toggle.isOn = !DisplayHighscores.LoadHighScores;
            DisplayHighscores.LoadHighScores = toggle.isOn;
        }
        public void GetGameVersion()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(),"Version.txt");
            if(File.Exists(path))
            {
                Version.text = File.ReadAllText(path);
            }
            else
                Version.gameObject.SetActive(false);
        }
    }

}
