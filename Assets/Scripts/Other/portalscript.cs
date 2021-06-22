#region

using player;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

// ReSharper disable All
namespace Other
{
    public class portalscript:MonoBehaviour
    {
        public string nextlevel;

        public void OnCollisionEnter ( Collision col )
        {
            if(col.gameObject.CompareTag("Player"))
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


        public IEnumerator LoadScene ()
        {
            if(nextlevel == "LevelSelect" || nextlevel == "Tutorial")
            {
                SceneManager.LoadScene(nextlevel);
            }

            if(name == "portal" && tag == "Fodder")
                GlobalVar.SingleLevel = false;
            else
                GlobalVar.SingleLevel = true;

            if(name.Contains("Level"))
                SceneManager.LoadScene($"Level{transform.GetChild(0).name.ToString()}");



            yield return new WaitForSeconds(0.01f);
            PlayerController.Endlv.SetActive(true);
            GameObject.Find("Player").GetComponent<PlayerController>().health = 5;
            yield return new WaitForSeconds(0.05f);
            if(nextlevel == "Final")
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
                    SceneManager.LoadScene("LevelSelect",LoadSceneMode.Single);
                }
            }
            else
            {
                try
                {
                    GameObject.Find("EventSystem").GetComponent<Timer>()
                        .AddScore(short.Parse(transform.GetChild(0).name.ToString()),nextlevel);
                }
                catch // This catch is done for me when i have a NULL name
                {
                    if(GlobalVar.SingleLevel)
                    {
                        SceneManager.LoadScene("LevelSelect");
                    }
                    else
                    {
                        SceneManager.LoadScene(nextlevel,LoadSceneMode.Single);
                    }
                }
            }
        }
    }
}