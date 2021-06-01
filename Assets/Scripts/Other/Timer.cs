using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ReSharper disable All
namespace Other
{
    public class Timer : MonoBehaviour
    {
        private static double[] _timeList = new double[100];
        private float _timer = 0;
        private Text _timertxt;

        void Awake()
        {
            _timertxt = GameObject.Find("Timer").GetComponent<Text>();
        }

        private void Start()
        {
            TimerManager.gamePlaying = true;
            if (TimerManager.gamePlaying)
                _timertxt.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (TimerManager.gamePlaying)
            {
                _timer += Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            _timertxt.text = $"{Math.Round(_timer, 2)}";
        }

        public void AddScore(short lvname, string nextlevel)
        {
            _timeList[lvname] = _timer;
            Saving.Saving.CheckLevelTime(GlobalVar.Name, _timer, lvname);
            SceneManager.LoadScene(nextlevel, LoadSceneMode.Single);
        }

        public void AddScore(short lvname)
        {
            if (string.IsNullOrEmpty(GlobalVar.Name)) return;
            Saving.Saving.CheckLevelTime(GlobalVar.Name, _timer, lvname);
        }

        public void FinalTime(short lvname)
        {
            if (string.IsNullOrEmpty(GlobalVar.Name))
                SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
            _timeList[lvname] = _timer;
            double finaltime = 0;
            foreach (var time in _timeList)
                finaltime += time;
            Saving.Saving.CheckLevelTime(GlobalVar.Name, finaltime, 0);
            SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
        }
    }
}