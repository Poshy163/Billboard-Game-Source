using Other;
using UnityEngine;
using System.Collections;
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
                StartCoroutine(LoadScene());
            } 
            catch { }
           
        }
    }


    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.01f);
        PlayerController.Endlv.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        try
        {
            GameObject.Find("EventSystem").GetComponent<Timer>().AddScore(short.Parse(transform.GetChild(0).name.ToString()),nextlevel);
        }
        catch { SceneManager.LoadScene(nextlevel,LoadSceneMode.Single); }
    }
}