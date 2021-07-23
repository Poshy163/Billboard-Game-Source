#region

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class Tutorial : MonoBehaviour
{
    private readonly string[] first =
    {
        "Here you will learn how to play",
        "The objective is to kill all of the minions",
        "They look like this",
        "Now, lets get you moving"
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
        StartCoroutine(NewTxt());
    }

    private IEnumerator NewTxt()
    {
        _firstLoop++;
        if (_firstLoop >= 4)
        {
            StartCoroutine(NextTxt("ADAD"));
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

    private IEnumerator NextTxt(string formatTxt)
    {
        yield return new WaitForSeconds(5);
        HelpTxt.text = formatTxt;
    }
}