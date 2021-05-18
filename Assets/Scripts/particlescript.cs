using UnityEngine;

public class particlescript : MonoBehaviour
{
    public float lifetimevalue;

    private void Start()
    {
        Destroy(gameObject, lifetimevalue);
    }
}