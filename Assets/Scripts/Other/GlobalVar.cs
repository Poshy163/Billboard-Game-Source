using UnityEngine;

namespace Other
{
    public class GlobalVar:MonoBehaviour
    {
        public enum GameDifficultyEnum
        {
            Easy,
            Normal,
            Hard
        }

        public static int ShootChance = 3; //1-5 smaller the lower chance
        public const int amountOfLevels = 4;
        public static string Name = null;
        public static GameDifficultyEnum GameDifficulty;

        //True means it wont attack the player
        public static bool Enemydontattack = true;
        public static float BulletSpeed = 5f;
        public static float SlowModeRegenRate = 25f;
        public static float SlowModeDrainRate = 400f;

        public static void UpdateSettings ()
        {
            switch(GameDifficulty)
            {
                case GameDifficultyEnum.Easy:
                    Enemydontattack = true;
                    ShootChance = 0;
                    break;
                case GameDifficultyEnum.Normal:
                    Enemydontattack = false;
                    BulletSpeed = 4.5f;
                    ShootChance = 3;
                    break;
                case GameDifficultyEnum.Hard:
                    Enemydontattack = false;
                    BulletSpeed = 6f;
                    ShootChance = 4;
                    break;
            }
        }
    }
}