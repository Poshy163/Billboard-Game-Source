using UnityEngine;
// ReSharper disable All
public class shurikenscript : MonoBehaviour
{
    private GameObject player;

    private Rigidbody rb;

    public float speed;

    private Color kickedcolor;

    private Color matcolor;

    private TrailRenderer trail;

    [HideInInspector] public GameObject spawnedby;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        transform.LookAt(player.transform);
        kickedcolor = new Color(0f, 214f, 212f);
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        rb.velocity = transform.forward * speed;
    }

    public void Kickedback(Vector3 dir)
    {
        gameObject.layer = LayerMask.NameToLayer("shurikenkicked");
        trail.startColor = new Color(0f, 214f, 212f);
        trail.endColor = new Color(0f, 214f, 212f);
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = kickedcolor;
        transform.LookAt(spawnedby.transform);
        speed = 20.45f;
    }

    private void OnCollisionEnter(Collision col)
    {
        Destroy(gameObject, 0f);
    }
}