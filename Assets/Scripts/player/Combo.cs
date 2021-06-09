using UnityEngine;
// ReSharper disable All

#pragma warning disable 414

namespace player
{
    public class Combo : MonoBehaviour
    {
        private static int _currentCombo;
        public static void ResetCombo()
        {
            _currentCombo = 0;
            ChangedCombo();
        }

        public static void AddToCombo(int addCombo = 1)
        {
            _currentCombo += addCombo;
            ChangedCombo();
        }

        private static void ChangedCombo()
        {
            if (_currentCombo >= 3)
                Debug.Log($"Current Combo is {_currentCombo}");
        }
    }
}