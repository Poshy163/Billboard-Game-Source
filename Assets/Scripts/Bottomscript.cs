using UnityEngine;
// ReSharper disable All
public class Bottomscript : MonoBehaviour
{

    private void OnCollisionEnter(Collision col)
    {
        Destroy(col.gameObject, 0f);
    }
}