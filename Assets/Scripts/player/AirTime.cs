using Other;
using UnityEngine;

public class AirTime : MonoBehaviour
{
    public static float _currentAirtime;
    private static bool GetTimeAir = true;

    private void Update()
    {
        if (GetTimeAir) _currentAirtime += Time.deltaTime;
    }

    public static void GetTime()
    {
        GetTimeAir = false;
        var time = _currentAirtime;
        _currentAirtime = 0;
        GlobalVar.CheckMaxAirtime(time);
    }

    public static void StartTime()
    {
        GetTimeAir = true;
    }
}