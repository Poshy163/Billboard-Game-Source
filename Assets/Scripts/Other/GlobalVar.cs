#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

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

        public const int AmountOfLevels = 4;
        public const float SlowModeRegenRate = 25f;
        public const float SlowModeDrainRate = 400f;
        public static float FloatingSpeed = 4f;

        public static bool SingleLevel = false;
        public static int Maxcombo;
        public static float MaxAirTime;
        public static int ShootChance = 3; //1-5 smaller the lower chance
        public static string Name = null;
        public static string DevName = "Dev";
        public static bool IsSignUp;

        public static GameDifficultyEnum GameDifficulty;

        //True means it wont attack the player
        public static bool Enemydontattack = true;
        public static float BulletSpeed = 5f;
        private static Dictionary<string,float> _playerStats;

        public static void UpdateUserStats ()
        {
            if(Name == null)
            {
                return;
            }

            if(IsSignUp)
            {
                Database.SendDummyInfo(Name);
                IsSignUp = false;
                _playerStats = Database.GetUserStats(Name);
            }
            else
            {
                _playerStats = Database.GetUserStats(Name);
                Maxcombo = (int)_playerStats["MaxCombo"];
                MaxAirTime = _playerStats["Max AirTime"];
            }
        }

        public static void CheckStats ()
        {
            if(IsSignUp)
            {
                return;
            }

            if(Maxcombo > (int)_playerStats["MaxCombo"] ||
                MaxAirTime > _playerStats["MaxCombo"]) // This is where the checks are made
            {
                Database.UpdateTopStats(Name);
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void CheckMaxCombo ( int combo )
        {
            if(combo > Maxcombo)
            {
                Maxcombo = combo;
            }
        }

        public static void CheckMaxAirtime ( float airTime )
        {
            if(airTime > MaxAirTime)
            {
                MaxAirTime = airTime;
            }
        }
    }
}