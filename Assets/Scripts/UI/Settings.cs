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
    public class Settings:MonoBehaviour
    {
        public GameObject panel;
        public TMP_Text debugtxt;

        public void Login ()
        {
            string name = GameObject.Find("NameLogin").GetComponent<TMP_InputField>().text.Trim();
            string password = GameObject.Find("PasswordLogin").GetComponent<TMP_InputField>().text.Trim();
            if(Saving.Saving.Login(name,password))
            {
                GlobalVar.Name = name;
                debugtxt.text = "Done Login";
                SceneManager.LoadScene("LevelSelect");

            }
            else
                debugtxt.text = "Either this name doesnt exist or you have the wrong password";
        }

        public void SignUp ()
        {
            string name = GameObject.Find("NameSignUp").GetComponent<TMP_InputField>().text.Trim();
            string password = GameObject.Find("PasswordSignUp").GetComponent<TMP_InputField>().text.Trim();
            if(Saving.Saving.SignUp(name,password))
            {
                GlobalVar.Name = name;
                debugtxt.text = "Done Sign up";
                SceneManager.LoadScene("LevelSelect");
            }
            else
                debugtxt.text = "This name is already in use";
        }

        public static void PermDeleteAccount ()
        {
            for(short i = 0;i <= 25;i++)
            {
                try
                {
                    Saving.Saving.DeleteDatabaseEntry(GlobalVar.Name,i);
                }
                catch { }
            }
            Saving.Saving.DeleteUser(GlobalVar.Name);
            SceneManager.LoadScene("Settings");

        }

        public void Start()
        {
            //if(!Application.isEditor)
            CheckVersionNumber();
        }

        void CheckVersionNumber()
        {
            var FileName = "NewVersion.zip";
            //TODO add version checking here
            var Install = true;

            if (!Install) return;
            //panel.SetActive(true);
            using (var client = new WebClient())
            {
                client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +  "Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.DownloadFile(
                    "https://api.github.com/repos/Poshy163/CryptoAPI/zipball",
                    FileName);
            }
        
            var zipPath =  Directory.GetCurrentDirectory() + @"\" + FileName;
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());
            ZipFile.ExtractToDirectory(zipPath, di.Parent.ToString());
            File.Delete(zipPath);
            Application.Quit();

        }


       
    }
}
