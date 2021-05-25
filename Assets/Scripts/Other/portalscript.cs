using Other;
using UnityEngine;
using System.Collections;
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
        GameObject.Find("EventSystem").GetComponent<Timer>().AddScore(transform.GetChild(0).name.ToString(), nextlevel);
    }
}