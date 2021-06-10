using UnityEngine;
using UnityEngine.UI;
// ReSharper disable All

#pragma warning disable 414

namespace player
{
    public class Combo:MonoBehaviour
    {
        private static Text Combotxt;
        private static float CurrentCooldown;
        private void Start ()
        {
            Combotxt = GameObject.Find("ComboTxt").GetComponent<Text>();
        }

        private static int _currentCombo;
        public static void ResetCombo ()
        {
            Other.GlobalVar.CheckMaxCombo(_currentCombo);
            _currentCombo = 0;
            Combotxt.text = null;
        }

        public static void AddToCombo ( int addCombo = 1 )
        {
            _currentCombo += addCombo;
            ChangedCombo();
        }

        private static void ChangedCombo ()
        {
            if(_currentCombo < 3)
            {
                return;
            }

            CurrentCooldown = 3;
            Combotxt.text = $"{_currentCombo}x combo";
        }


        private void Update ()
        {
            CurrentCooldown -= Time.deltaTime;
            if(CurrentCooldown <= 0)
            {
                Combotxt.text = null;
            }
        }

    }
}