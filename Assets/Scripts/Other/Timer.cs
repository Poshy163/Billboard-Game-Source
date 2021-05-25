using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable All
namespace Other
{
    public class Timer : MonoBehaviour
    {
        private float _timer = 0;
        private Text _timertxt;
        private static double[] _timeList = new double[100];
        void Start()
        {
            _timertxt = GameObject.Find("Timer").GetComponent<Text>();
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
            _timertxt.text = $"{Math.Round(_timer,2)}";
        }

        public void AddScore (short lvname,string nextlevel)
        {
             _timeList[lvname] = _timer;
             Saving.Saving.CheckLevelTime(GlobalVar.Name,_timer,lvname);
             SceneManager.LoadScene(nextlevel,LoadSceneMode.Single);
        }

        public void FinalTime( short lvname )
        {
            _timeList[lvname] = _timer;
            double finaltime = 0;
            foreach(double time in _timeList)
                finaltime += time;
            Saving.Saving.CheckLevelTime(GlobalVar.Name,finaltime,0);
            SceneManager.LoadScene("LevelSelect",LoadSceneMode.Single);
        }
    }
}
