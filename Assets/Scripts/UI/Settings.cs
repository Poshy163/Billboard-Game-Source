using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable PossibleNullReferenceException

namespace UI
{
    public class Settings : MonoBehaviour
    {

        public GameObject panel;
        public TMP_Text debugtxt;

        public void Login()
        {
            var localname = GameObject.Find("NameLogin").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordLogin").GetComponent<TMP_InputField>().text.Trim();
            if (Saving.Saving.Login(localname, password))
            {
                GlobalVar.Name = localname;
                debugtxt.text = "Done Login";
                SceneManager.LoadScene("LevelSelect");

            }
            else
                debugtxt.text = "Either this name doesnt exist or you have the wrong password";
        }

        public void SignUp()
        {
            var localname = GameObject.Find("NameSignUp").GetComponent<TMP_InputField>().text.Trim();
            var password = GameObject.Find("PasswordSignUp").GetComponent<TMP_InputField>().text.Trim();
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
                catch {
                    //Ignored
                }
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
    }

}
