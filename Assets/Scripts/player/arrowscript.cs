using UnityEngine;

// ReSharper disable All

public class arrowscript:MonoBehaviour
{
    public float speed;

    public float speedincreaseduringslowmo;

    public bool cancausegrapple;

    [HideInInspector] public bool hit;
    private Rigidbody rb;

    private void Start ()
    {
        hit = false;
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject,2f);
    }

    private void Update ()
    {
        if(Time.timeScale < 0.5f)
        {
            rb.velocity = transform.forward * (speed + speedincreaseduringslowmo);
        }
        else
        {
            rb.velocity = transform.forward * (speed - 10f);
        }

        transform.Rotate(new Vector3(0f,0f,20f),Space.Self);
    }

    private void OnCollisionEnter ( Collision col )
    {
        Destroy(gameObject,0f);
    }
}