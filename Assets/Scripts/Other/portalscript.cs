#region

using Enemy;
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

            if(col.gameObject.CompareTag("arrow") && !col.gameObject.GetComponent<arrowscript>().hit)
            {
                col.gameObject.GetComponent<arrowscript>().hit = true;
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(1).GetComponent<greentargetscript>().arrowstate = true;
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enemytodashto = gameObject;
            }
        }


        public IEnumerator LoadScene ()
        {
            if(nextlevel == "Lobby" || nextlevel == "Tutorial" || nextlevel == "Settings")
            {
                PlayerController.Endlv.SetActive(true);
                SceneManager.LoadScene(nextlevel);
            }

            if(name == "portal" && tag == "Fodder")
            {
                GlobalVar.SingleLevel = false;
            }
            else if(tag == "Fodder")
            {
                GlobalVar.SingleLevel = true;
            }

            if(name.Contains("Level"))
            {
                string lv = transform.GetChild(0).name.ToString();
                if(int.Parse(lv) == 4)
                {
                    SceneManager.LoadScene("Bossfight");
                }
                else
                {
                    SceneManager.LoadScene($"Level{transform.GetChild(0).name.ToString()}");
                }
            }


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
                    SceneManager.LoadScene("Lobby",LoadSceneMode.Single);
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
                        SceneManager.LoadScene("Lobby");
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