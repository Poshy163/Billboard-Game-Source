using UnityEngine;

// ReSharper disable All
//Formatted
public class Bottomscript:MonoBehaviour
{
    public void OnTriggerEnter ( Collider col )
    {
        if(col.tag != "Player")
        {
            Destroy(col.gameObject,0f);
        }
    }
}