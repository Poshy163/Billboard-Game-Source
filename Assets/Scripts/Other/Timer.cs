using System;
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

        public void AddScore(string name)
        {
            TimerManager.OnLevelComplete(int.Parse(name),_timer);
        }
    }
}
