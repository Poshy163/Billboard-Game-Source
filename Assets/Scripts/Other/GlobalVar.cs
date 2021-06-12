using System;
using System.Collections.Generic;
using UnityEngine;

namespace Other
{
    public class GlobalVar : MonoBehaviour
    {
        public enum GameDifficultyEnum
        {
            Easy,
            Normal,
            Hard
        }

        public const int AmountOfLevels = 4;
        public const float SlowModeRegenRate = 25f;
        public const float SlowModeDrainRate = 400f;
        public static int Maxcombo;
        public static int ShootChance = 3; //1-5 smaller the lower chance
        public static string Name = null;

        public static GameDifficultyEnum GameDifficulty;

        //True means it wont attack the player
        public static bool Enemydontattack = true;
        public static float BulletSpeed = 5f;
        private static Dictionary<string, float> _playerStats;

        public static void UpdateUserStats()
        {
            _playerStats = Saving.Saving.GetUserStats(Name);
            Maxcombo = (int) _playerStats["MaxCombo"];
        }

        public static void CheckStats()
        {
            if (Maxcombo > (int) _playerStats["MaxCombo"])
                Saving.Saving.UpdateTopStats(Name);
        }


        public static void UpdateSettings()
        {
            switch (GameDifficulty)
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void CheckMaxCombo(int combo)
        {
            if (combo > Maxcombo) Maxcombo = combo;
        }
    }
}