#region

using UnityEngine;

#endregion

// ReSharper disable All
//Formatted
namespace Enemy
{
    public class greentargetscript:MonoBehaviour
    {
        public GameObject greenarrow;

        [HideInInspector] public bool arrowstate;

        public void Start ()
        {
            arrowstate = false;
        }

        public void Update ()
        {
            greenarrow.SetActive(arrowstate);
        }

        public void SetArrowstate ()
        {
            arrowstate = !arrowstate;
        }
    }
}