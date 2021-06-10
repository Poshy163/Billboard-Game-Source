using UnityEngine;
using System.Collections.Generic;

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
        public const int amountOfLevels = 4;
        public static int Maxcombo = 0;
        public static int ShootChance = 3; //1-5 smaller the lower chance
        public static string Name = null;
        public static GameDifficultyEnum GameDifficulty;
        //True means it wont attack the player
        public static bool Enemydontattack = true;
        public static float BulletSpeed = 5f;
        public static float SlowModeRegenRate = 25f;
        public static float SlowModeDrainRate = 400f;
        public static Dictionary<string,float> PlayerStats;
        public static void UpdateUserStats()
        {
           PlayerStats = Saving.Saving.GetUserStats(Name);
           Maxcombo = (int)PlayerStats["MaxCombo"];
        }

        public static void CheckStats()
        {
            if(Maxcombo > (int)PlayerStats["MaxCombo"])
            { 
                Saving.Saving.AddTopStats(Name);
            }
                
        }

        
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
        public static void CheckMaxCombo ( int _combo )
        {
            if(_combo > Maxcombo)
            {
                Maxcombo = _combo;
            }
        }
    }
}