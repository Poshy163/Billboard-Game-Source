using Other;
using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable All
public class portalscript : MonoBehaviour
{
    public string nextlevel;
    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            try
            { 
                GameObject.Find("EventSystem").GetComponent<Timer>().AddScore(transform.GetChild(0).name);
            } catch { }
            SceneManager.LoadScene(nextlevel, LoadSceneMode.Single);
        }
    }
}