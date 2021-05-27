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
}
