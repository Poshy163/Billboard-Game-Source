using UnityEngine;
using static Saving.Saving;

namespace UI
{
    public class DisplayHighscores : MonoBehaviour
    {
        private void Start()
        { 
            var times = GetTopTimes("Joshua", 1);
           //Debug.Log(times.ElementAt(0).Key+"," + times.ElementAt(0).Value);
           //Just a proof of concept 
        }
    }
}
