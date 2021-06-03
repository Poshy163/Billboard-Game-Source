using UnityEngine;

// ReSharper disable All
public class particlescript:MonoBehaviour
{
    public float lifetimevalue;

    private void Start ()
    {
        Destroy(gameObject,lifetimevalue);
    }
}