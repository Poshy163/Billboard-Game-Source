#region

using Enemy;
using player;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 652

#endregion

public class Tutorial:MonoBehaviour
{
    private readonly string[] firstTxt =
    {
        "Here you will learn how to play",
        "The objective is to kill all of the minions and advance to the next level",
        "The minions look like this",
        "Now, lets get you movingTxt"
    };

    private readonly string[] movingTxt =
    {
        "To move around, use WASD, use the spacebar to jump",
        "Now, lets see how much you have learnt\n Dodge this enemy!",
        "",
        "Good work!"
    };

    private readonly string[] shootingTxt =
    {
        "Now, lets move you on to shooting!",
        "To shoot, Click your left mouse button down.",
        "Try it out!",
        "Nice work!\n Now shoot this enemy!",
        "Good Work!",
    };

    private readonly string[] slowMoTxt =
    {
        "Now, lets teach you about slow-mode",
        "To enter this mode, click the right mouse button, or E",
        "That bar at the top is how much energy you have got \n You can exit and enter at any time",
    };

    private readonly string[] EndTxt =
    {
        "This is all you need to know now",
        "There is many more tricks to learn which you will relise as you play",
        "Good Luck!, enter the portal to return to the menu.",
        ""

    };


    private GameObject Portal;
    private GameObject dummy;
    private Text HelpTxt;

    private void Start ()
    {
        dummy = GameObject.Find("Gargoyle");
        dummy.SetActive(false);
        Portal = GameObject.Find("TPportal");
        Portal.SetActive(false);
        HelpTxt = GameObject.Find("HelpTxt").GetComponent<Text>();
        HelpTxt.text = "Welcome to the Tutorial!";
        StartTutorial();
    }

    private void StartTutorial ()
    {
        PlayerController.PlayerMove = false;
        StartCoroutine(Shooting());
    }

    private IEnumerator IntroTxt ()
    {
        for(var i = 0;i <= 10;i++)
        {
            if(i >= 4)
            {
                PlayerController.PlayerMove = true;
                StartCoroutine(Moving());
                yield break;
            }
            yield return new WaitForSeconds(5);

            HelpTxt.text = firstTxt[i];

            if(i == 2)
            {
                dummy.SetActive(true);
                yield return new WaitForSeconds(5);
                i++;
                HelpTxt.text = firstTxt[i];
                dummy.SetActive(false);
            }
            continue;
        }
    }

    private IEnumerator Moving ()
    {
        for(var i = 0;i < 4;i++)
        {
            HelpTxt.text = movingTxt[i];
            switch(i)
            {
                case 2:
                    dummy.transform.position = new Vector3(0,-4.652f,-10.19f);
                    dummy.SetActive(true);
                    for(var j = 0;j < 100;j++)
                    {
                        yield return new WaitForSeconds(0.05f);
                        dummy.transform.position = Vector3.Lerp(dummy.transform.position,
                            new Vector3(0,-4.652f,-34.66f),0.02f);
                    }

                    dummy.SetActive(false);
                    HelpTxt.text = movingTxt[i + 1];
                    break;
            }

            yield return new WaitForSeconds(5);
        }

        StartCoroutine(Shooting());
    }


    private IEnumerator Shooting ()
    {
        PlayerController.PlayerMove = false;
        for(var i = 0;i < 5;i++)
        {
            HelpTxt.text = shootingTxt[i];

            switch(i)
            {
                case 2:
                    PlayerController.PlayerMove = true;
                    break;
                case 3:
                    dummy.SetActive(true);
                    dummy.GetComponent<gargoylescript>().health = 10f;
                    dummy.transform.position = new Vector3(0,-3.72f,-7.86f);
                    for(var j = 0;j < long.MaxValue;j++)
                    {
                        yield return new WaitForSeconds(0.1f);
                        if(GameObject.Find("Gargoyle"))
                        {
                            continue;
                        }
                        HelpTxt.text = shootingTxt[i + 1];
                        break;
                    }

                    break;
                case 4:
                    StartCoroutine(SlowMode());
                    break;
            }

            yield return new WaitForSeconds(5);
        }
    }

    private IEnumerator SlowMode ()
    {
        for(var i = 0;i < 10;i++)
        {
            HelpTxt.text = slowMoTxt[i];
            switch(i)
            {
                case 1:
                    PlayerController.PlayerMove = true;
                    for(var j = 0;j < long.MaxValue;j++)
                    {
                        if(!Input.GetMouseButtonDown(1))
                        {
                            yield return new WaitForSeconds(0.000001f);
                        }
                        else
                        {
                            HelpTxt.text = slowMoTxt[i + 1];
                            break;
                        }
                    }
                    break;

                case 2:
                    yield return new WaitForSeconds(5);
                    StartCoroutine(End());
                    break;

            }

            yield return new WaitForSeconds(5);
        }

        yield return new WaitForSeconds(3);
    }


    private IEnumerator End()
    {
        for(var i = 0;i < 4;i++)
        {
            HelpTxt.text = EndTxt[i];
            switch(i)
            {
                case 2:
                    Portal.SetActive(true);
                    break;
                case 3:
                    PlayerController.PlayerMove = true;
                    break;
                    
            }
            yield return new WaitForSeconds(5);
        }


        yield break;
    }
}