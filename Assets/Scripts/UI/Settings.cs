#region

using Other;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace UI
{
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
            MainMenu.PermDeleteAccount();
        }

        public void ClearUserScore ()
        {
            if(string.IsNullOrEmpty(GlobalVar.Name))
            {
                return;
            }
            Database.ResetStats();
            SceneManager.LoadScene("Lobby");
        }

        public void Back ()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}