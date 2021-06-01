using UnityEngine;
using static Saving.Saving;
using TMPro;
using System;
using Other;

namespace UI
{
    public class DisplayHighscores : MonoBehaviour
    {
        public static bool LoadHighScores = true;
        private void Start()
        {
            if(LoadHighScores)
                LoadHighScore();
            else
                UnloadHighScore();
        }
        void LoadHighScore()
        {
            for(int z = 0;z <= GlobalVar.amountOfLevels;z++)
            {
                var times = GetTopTimes("Joshua",(short)z);
                GameObject gme = GameObject.Find($"Level {z}");
                GameObject panel = gme.transform.GetChild(0).gameObject;
                if(gme.name == "Level 0")
                    gme.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "Overall Time";
                else
                    gme.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = gme.name;

                for(int i = 0;i <= 4;i++) //This is 5 
                {
                    TMP_Text nametxt = panel.transform.GetChild(i).gameObject.GetComponent<TMP_Text>();
                    TMP_Text timetxt = panel.transform.GetChild(i + 5).gameObject.GetComponent<TMP_Text>();
                    try
                    {
                        nametxt.text = $"{i + 1}. {times[i].Key}";
                        timetxt.text = $"{Math.Round(times[i].Value,2)}s";
                    }
                    catch
                    {
                        nametxt.text = $"{i + 1}. Null";
                        timetxt.text = "NULL";
                    }
                }
            }
        }
        void UnloadHighScore()
        {
            for(int i = 0; i <= GlobalVar.amountOfLevels;i++)
            {
                GameObject gme = GameObject.Find($"Level {i}");
                gme.SetActive(false);
            }
        }
    }
}
