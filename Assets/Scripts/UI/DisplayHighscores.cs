using System;
using Other;
using TMPro;
using UnityEngine;
using static Saving.Saving;

// ReSharper disable IdentifierTypo

namespace UI
{
    public class DisplayHighscores : MonoBehaviour
    {
        public static bool LoadHighScores = true;

        private void Start()
        {
            if (LoadHighScores)
                LoadHighScore();
            else
                UnloadHighScore();
        }

        private static void LoadHighScore()
        {
            for (var z = 0; z <= GlobalVar.amountOfLevels; z++)
            {
                var times = GetTopTimes((short) z);
                var gme = GameObject.Find($"Level {z}");
                var panel = gme.transform.GetChild(0).gameObject;
                gme.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text =
                    gme.name == "Level 0" ? "Overall Time (All in one session)" : gme.name;

                for (var i = 0; i <= 3; i++) //This is 5, can be changed
                {
                    var nametxt = panel.transform.GetChild(i).gameObject.GetComponent<TMP_Text>();
                    var timetxt = panel.transform.GetChild(i + 5).gameObject.GetComponent<TMP_Text>();
                    try
                    {
                        nametxt.text = $"{i + 1}. {times[i].Key}";
                        timetxt.text = $"{Math.Round(times[i].Value, 2)}s";
                    }
                    catch
                    {
                        nametxt.text = $"{i + 1}. Null";
                        timetxt.text = "NULL";
                    }
                }

                var nametxtlast = panel.transform.GetChild(4).gameObject.GetComponent<TMP_Text>();
                var timetxtlast = panel.transform.GetChild(9).gameObject.GetComponent<TMP_Text>();
                nametxtlast.text = $"?.  {GlobalVar.Name}";
                timetxtlast.text = "No Time";
                short localIndex = 0;
                foreach (var var in times)
                {
                    localIndex++;
                    if (var.Key != GlobalVar.Name) continue;

                    nametxtlast.text = $"{localIndex}. {GlobalVar.Name}";
                    timetxtlast.text = $"{Math.Round(var.Value, 2)}s";
                    break;
                }
            }
        }

        private static void UnloadHighScore()
        {
            for (var i = 0; i <= GlobalVar.amountOfLevels; i++)
            {
                var gme = GameObject.Find($"Level {i}");
                gme.SetActive(false);
            }
        }
    }
}