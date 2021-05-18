using UnityEngine;
// ReSharper disable All
#pragma warning disable 649
#pragma warning disable 414


public class Floatingenemyscript : MonoBehaviour
{
    private GameObject player;

    private Color matcolor;

    private Color damagedcolor;

    private Animator anim;

    private Rigidbody rb;

    private Vector3 dir;

    [HideInInspector] public bool kicked;

    public GameObject enemysprite;

    public GameObject particles;

    public float speed;

    private void Start()
    {
        anim = enemysprite.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        kicked = false;
    }

    private void Awake()
    {
        anim = enemysprite.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        kicked = false;
    }

    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("MainCamera");
        var position = player.transform.position;
        var worldPosition = new Vector3(position.x, position.y,
            position.z);
        transform.LookAt(worldPosition);
        if (kicked)
        {
            rb.velocity = dir * (speed + 20f);
            return;
        }

        if (!(Vector3.Distance(transform.position, player.transform.position) > 0.3f)) return;
        var maxDistanceDelta = 0f;
        if (Time.timeScale < 0.5f && !kicked)
        {
            maxDistanceDelta = Time.deltaTime * (speed - 5.34f);
        }
        else if (Time.timeScale > 0.5f && !kicked)
        {
            maxDistanceDelta = Time.deltaTime * speed;
        }

        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, maxDistanceDelta);
    }

    public void enemykickedback(Vector3 direction)
    {
        gameObject.layer = LayerMask.NameToLayer("Kicked Enemy");
        anim.SetTrigger("kicked");
        kicked = true;
        dir = direction;
        Invoke("Destroythehead", 4.89f);
    }

    public void takendamage()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = damagedcolor;
        Invoke("resetcolor", 0.098f);
    }

    private void resetcolor()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = matcolor;
    }

    private void Destroythehead()
    {
        Destroy(gameObject, 0f);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (kicked)
        {
            Instantiate(particles, transform.position, Quaternion.identity).transform.forward = transform.forward;
            Destroy(gameObject, 0f);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Kicked Enemy"))
        {
            takendamage();
            enemykickedback(transform.forward * -1f);
            if (col.gameObject.tag == "Floating enemy")
            {
                Destroy(col.gameObject, 0f);
            }
        }

        if (col.gameObject.tag == "arrow")
        {
            if (col.gameObject.GetComponent<arrowscript>().cancausegrapple &&
                !transform.GetComponent<greentargetscript>().arrowstate)
            {
                gameObject.GetComponent<greentargetscript>().SetArrowstate();
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enemytodashto = gameObject;
            }

            rb.velocity = new Vector3(0f, 0f, 0f);
        }
    }
}