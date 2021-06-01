using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable All
#pragma warning disable 414
public class Camerascript : MonoBehaviour
{
    private Camera cam;

    private float curtime;

    public GameObject portal;

    private bool playedwonsound;

    private bool canchecknow;

    private void Start()
    {
        portal = GameObject.Find("portal");
        cam = GetComponent<Camera>();
        Cursor.visible = false;
        curtime = 1f;
        playedwonsound = false;
        canchecknow = false;
        Invoke("startchecking", 1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = (Time.timeScale != 0f) ? 0 : 1;
        }

        if (canchecknow && GameObject.FindGameObjectsWithTag("Gargoyle").Length == 0 &&
            GameObject.FindGameObjectsWithTag("Summoner").Length == 0)
        {
            portal.SetActive(true);
            if (!playedwonsound)
            {
                playedwonsound = true;
                soundmanagerscript.playsound("won");
            }
        }
        else
        {
            portal.SetActive(false);
        }
    }

    private void startchecking()
    {
        canchecknow = true;
    }

    public void camershake(float amount)
    {
        cam.fieldOfView = amount;
        Invoke("resetcamera", 0.04f);
    }

    private void resetcamera()
    {
        cam.fieldOfView = 60f;
    }
}