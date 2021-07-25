#region

using System.Collections;
using Enemy;
using player;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class Tutorial : MonoBehaviour
{
    private readonly string[] first =
    {
        "Here you will learn how to play",
        "The objective is to kill all of the minions and advance to the next level",
        "They look like this",
        "Now, lets get you moving"
    };

    private readonly string[] moving =
    {
        "To move around, use WASD. And use the spacebar to jump",
        "Now, lets see how much you have learnt\n Dodge this enemy!",
        "",
        "Good work!"
    };

    private readonly string[] shooting =
    {
        "Now, lets move you on to shooting!",
        "To shoot, Click your left mouse button down.",
        "Try it out!",
        "Nice work!\n Now shoot this enemy!",
        "Good Work!",
        ""
    };

    private readonly string[] slowMo =
    {
        "Now, lets teach you about slow-mode",
        "To enter this mode, click the right mouse button, or E",
        ""
    };


    private int _firstLoop;
    private GameObject dummy;
    private Text HelpTxt;

    private void Start()
    {
        dummy = GameObject.Find("Gargoyle");
        dummy.SetActive(false);
        HelpTxt = GameObject.Find("HelpTxt").GetComponent<Text>();
        HelpTxt.text = "Welcome to the Tutorial!";
        StartTutorial();
    }

    private void StartTutorial()
    {
        PlayerController.PlayerMove = false;
        StartCoroutine(Shooting());
    }

    private IEnumerator NewTxt()
    {
        _firstLoop++;
        if (_firstLoop >= 4)
        {
            PlayerController.PlayerMove = true;
            StartCoroutine(Moving());
            yield break;
        }

        yield return new WaitForSeconds(5);


        HelpTxt.text = first[_firstLoop];

        if (_firstLoop == 2)
        {
            dummy.SetActive(true);
            yield return new WaitForSeconds(5);
            _firstLoop++;
            HelpTxt.text = first[_firstLoop];
            dummy.SetActive(false);
        }

        StartCoroutine(NewTxt());
    }

    private IEnumerator Moving()
    {
        for (var i = 0; i < 4; i++)
        {
            HelpTxt.text = moving[i];
            switch (i)
            {
                case 2:
                    dummy.transform.position = new Vector3(0, -4.652f, -10.19f);
                    dummy.SetActive(true);
                    for (var j = 0; j < 100; j++)
                    {
                        yield return new WaitForSeconds(0.05f);
                        dummy.transform.position = Vector3.Lerp(dummy.transform.position,
                            new Vector3(0, -4.652f, -34.66f), 0.02f);
                    }

                    dummy.SetActive(false);
                    HelpTxt.text = moving[i + 1];
                    break;
            }

            yield return new WaitForSeconds(3);
        }

        StartCoroutine(Shooting());
    }


    private IEnumerator Shooting()
    {
        PlayerController.PlayerMove = false;
        for (var i = 0; i < 6; i++)
        {
            HelpTxt.text = shooting[i];

            switch (i)
            {
                case 2:
                    PlayerController.PlayerMove = true;
                    break;
                case 3:
                    dummy.SetActive(true);
                    dummy.GetComponent<gargoylescript>().health = 1;
                    dummy.transform.position = new Vector3(0, -3.72f, -7.86f);
                    for (var j = 0; j < 10000; j++)
                    {
                        yield return new WaitForSeconds(0.1f);
                        if (GameObject.Find("Gargoyle")) continue;
                        HelpTxt.text = shooting[i + 1];
                        break;
                    }

                    break;
                case 5:
                    StartCoroutine(SlowMode());
                    break;
            }

            yield return new WaitForSeconds(5);
        }
    }

    private IEnumerator SlowMode()
    {
        for (var i = 0; i < 4; i++)
        {
            HelpTxt.text = slowMo[i];
            yield return new WaitForSeconds(3);
        }

        yield return new WaitForSeconds(3);
    }
}