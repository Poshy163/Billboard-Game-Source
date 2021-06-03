using System.Collections;
using Other;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable All
public class portalscript : MonoBehaviour
{
    public string nextlevel;

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            try
            {
                GameObject.Find("Player").GetComponent<PlayerController>().health = 5;
                StartCoroutine(LoadScene());
            }
            catch
            { }
        }
    }


    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.01f);
        PlayerController.Endlv.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        if (nextlevel == "Final")
        {
            try
            {
                GameObject.Find("EventSystem").GetComponent<Timer>()
                    .AddScore(short.Parse(transform.GetChild(0).name.ToString()));
                GameObject.Find("EventSystem").GetComponent<Timer>()
                    .FinalTime(short.Parse(transform.GetChild(0).name.ToString()));
            }
            catch
            {
                SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
            }
        }
        else
        {
            try
            {
                GameObject.Find("EventSystem").GetComponent<Timer>()
                    .AddScore(short.Parse(transform.GetChild(0).name.ToString()), nextlevel);
            }
            catch
            {
                SceneManager.LoadScene(nextlevel, LoadSceneMode.Single);
            }
        }
    }
}