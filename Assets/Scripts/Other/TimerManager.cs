using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable All

namespace Other
{
    public class TimerManager : MonoBehaviour
    {
        private static float[] _timeList = new float[100];
        public static bool gamePlaying = true;
        private Text _timertxt;


        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static void OnLevelComplete(int level, float time)
        {
            if(_timeList[level] == 0)
            {
                _timeList[level] = time;
            }
            else if (_timeList.ElementAt(level) < time)
            {
                _timeList[level] = time;
            }
        }
    }
}
