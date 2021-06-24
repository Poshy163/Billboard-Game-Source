using Other;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings:MonoBehaviour
{
    private void Start ()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ClearUserData ()
    {
        if(string.IsNullOrEmpty(GlobalVar.Name))
        {
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        MainMenu.PermDeleteAccount();
    }

    public void ClearUserScore ()
    {
        Saving.Saving.ResetStats();
        SceneManager.LoadScene("Lobby");
    }

    public void Back ()
    {
        SceneManager.LoadScene("Lobby");
    }


}
