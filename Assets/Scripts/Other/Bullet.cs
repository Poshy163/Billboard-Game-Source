using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float DeathTimer = 2f;

    private void Start()
    {
        Destroy(gameObject, DeathTimer);
    }
}