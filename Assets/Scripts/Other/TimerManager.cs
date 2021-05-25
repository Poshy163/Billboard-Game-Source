using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    }
}
