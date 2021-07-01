#region

using UnityEngine;

#endregion

// ReSharper disable All
//Formatted
namespace Other
{
    public class Bottomscript : MonoBehaviour
    {
        public void OnTriggerEnter(Collider col)
        {
            if (col.tag != "Player")
            {
                Destroy(col.gameObject, 0f);
            }
        }
    }
}