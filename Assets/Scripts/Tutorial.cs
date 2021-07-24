#region

using System.Collections;
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
        "To move around, use WASD.",
        "Now, lets see how much you have learnt\n Dodge this enemy!",
        "",
        "Good work!"
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
        StartCoroutine(NewTxt());
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

        HelpTxt.text = "DONE AT THIS POINT";
    }
}