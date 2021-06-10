using UnityEngine;

// ReSharper disable All
namespace Other
{
    public class particlescript:MonoBehaviour
    {
        public float lifetimevalue;

        private void Start ()
        {
            Destroy(gameObject,lifetimevalue);
        }
    }
}