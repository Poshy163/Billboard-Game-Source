using UnityEngine;
public class GlobalVar:MonoBehaviour
{
    public const int amountOfLevels = 4;
    public static string Name = null;
    public static GameDifficultyEnum GameDifficulty;

    //True means it wont attack the player
    public static bool Enemydontattack = true;
    public static float BulletSpeed = 5f;
    public static float SlowModeRegenRate = 25f;
    public static float SlowModeDrainRate = 400f;
    public enum GameDifficultyEnum
    {
        Easy,
        Normal,
        Hard
    }
    public static void UpdateSettings ()
    {
        switch(GameDifficulty)
        {
            case GameDifficultyEnum.Easy:
                Enemydontattack = true;
                break;
            case GameDifficultyEnum.Normal:
                Enemydontattack = false;
                BulletSpeed = 4f;
                break;
            case GameDifficultyEnum.Hard:
                Enemydontattack = false;
                BulletSpeed = 6f;
                break;
        }

    }


}
