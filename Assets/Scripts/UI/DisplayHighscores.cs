using Other;
using System;
using TMPro;
using UnityEngine;
using static Saving.Saving;

// ReSharper disable IdentifierTypo

namespace UI
{
    public class DisplayHighscores:MonoBehaviour
    {
        public static bool LoadHighScores = true;

        private void Start ()
        {
            if(LoadHighScores)
            {
                LoadHighScore();
            }
            else
            {
                UnloadHighScore();
            }
        }

        private static void LoadHighScore ()
        {
            for(int z = 0;z <= GlobalVar.amountOfLevels;z++)
            {
                System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string,float>> times = GetTopTimes((short)z);
                GameObject gme = GameObject.Find($"Level {z}");
                GameObject panel = gme.transform.GetChild(0).gameObject;
                gme.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text =
                    gme.name == "Level 0" ? "Overall Time (All in one session)" : gme.name;

                for(int i = 0;i <= 3;i++) //This is 5, can be changed
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

                TMP_Text nametxtlast = panel.transform.GetChild(4).gameObject.GetComponent<TMP_Text>();
                TMP_Text timetxtlast = panel.transform.GetChild(9).gameObject.GetComponent<TMP_Text>();
                nametxtlast.text = $"?.  {GlobalVar.Name}";
                timetxtlast.text = "No Time";
                short localIndex = 0;
                foreach(System.Collections.Generic.KeyValuePair<string,float> var in times)
                {
                    localIndex++;
                    if(var.Key != GlobalVar.Name)
                    {
                        continue;
                    }

                    nametxtlast.text = $"{localIndex}. {GlobalVar.Name}";
                    timetxtlast.text = $"{Math.Round(var.Value,2)}s";
                    break;
                }
            }
        }

        private static void UnloadHighScore ()
        {
            for(int i = 0;i <= GlobalVar.amountOfLevels;i++)
            {
                GameObject gme = GameObject.Find($"Level {i}");
                gme.SetActive(false);
            }
        }
    }
}