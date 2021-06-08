using UnityEngine;

// ReSharper disable All
namespace Enemy
{
    public class gargoyleanimationscript:MonoBehaviour
    {
        private gargoylescript gargoyle;

        private void Start ()
        {
            gargoyle = transform.parent.gameObject.GetComponent<gargoylescript>();
        }

        private void shoot ()
        {
            gargoyle.shoot();
        }
    }
}