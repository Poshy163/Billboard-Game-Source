using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable All
public class portalscript : MonoBehaviour
{
    public string nextlevel;
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(nextlevel, LoadSceneMode.Single);
        }
    }
}