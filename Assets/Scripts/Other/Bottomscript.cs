using UnityEngine;
// ReSharper disable All
//Formatted
public class Bottomscript : MonoBehaviour
{    public void OnCollisionEnter(Collision col)
    {
        Destroy(col.gameObject, 0f);
    }
}