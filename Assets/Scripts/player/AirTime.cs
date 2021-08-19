#region

using Other;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace player
{
    public class AirTime : MonoBehaviour
    {
        private static float _currentAirtime;
        private static bool _getTimeAir = true;

        private void Update()
        {
            if (_getTimeAir) _currentAirtime += Time.deltaTime;
        }

        public static void GetTime()
        {
            if (SceneManager.GetActiveScene().name == "Tutorial") return;

            _getTimeAir = false;
            var time = _currentAirtime;
            _currentAirtime = 0;
            GlobalVar.CheckMaxAirtime(time);
        }

        public static void StartTime()
        {
            _getTimeAir = true;
        }
    }
}