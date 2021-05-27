using System.Net;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings:MonoBehaviour
{

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

    public void Awake()
    {
        //if(!Application.isEditor)
            CheckVersionNumber();
    }

    void CheckVersionNumber()
    {
        string path = @"C:\Users\s210262\Desktop\TempBulid";
        DirectoryInfo di;
        //TODO add version checking here
        bool Install = true;

        if(Install)
        {

            ///Application.OpenURL("https://github.com/Poshy163/Billboard-Game");
            //Application.Quit();

            if(!Directory.Exists(path))
            {
                di = Directory.CreateDirectory(path);
            }

            // using(var client = new WebClient())
            // {
            //     client.DownloadFile("https://github.com/user/project/archive/v1.4.zip",@"L:\Program");
            // }


            downloadFile("https://api.github.com/repos/nodatime/nodatime/zipball",path);
        }

    }
    public static void downloadFile ( string sourceURL,string destinationPath )
    {
        Debug.Log("Downloading");
        long fileSize = 0;
        int bufferSize = 1024;
        bufferSize *= 1000;
        long existLen = 0;

        System.IO.FileStream saveFileStream;
        if(System.IO.File.Exists(destinationPath))
        {
            System.IO.FileInfo destinationFileInfo = new System.IO.FileInfo(destinationPath);
            existLen = destinationFileInfo.Length;
        }

        if(existLen > 0)
            saveFileStream = new System.IO.FileStream(destinationPath,
                                                      System.IO.FileMode.Append,
                                                      System.IO.FileAccess.Write,
                                                      System.IO.FileShare.ReadWrite);
        else
            saveFileStream = new System.IO.FileStream(destinationPath,
                                                      System.IO.FileMode.Create,
                                                      System.IO.FileAccess.Write,
                                                      System.IO.FileShare.ReadWrite);

        System.Net.HttpWebRequest httpReq;
        System.Net.HttpWebResponse httpRes;
        httpReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(sourceURL);
        httpReq.AddRange((int)existLen);
        System.IO.Stream resStream;
        httpRes = (System.Net.HttpWebResponse)httpReq.GetResponse();
        resStream = httpRes.GetResponseStream();

        fileSize = httpRes.ContentLength;

        int byteSize;
        byte[] downBuffer = new byte[bufferSize];

        while((byteSize = resStream.Read(downBuffer,0,downBuffer.Length)) > 0)
        {
            saveFileStream.Write(downBuffer,0,byteSize);
        }
        saveFileStream.Close();
    }

}
