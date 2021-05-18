using UnityEngine;
// ReSharper disable All

public class fodderenemyscript : MonoBehaviour
{
    private bool startattacking;

    private GameObject player;

    private Color matcolor;

    private Color damagedcolor;

    private Color almostdeadcolor;

    private Animator anim;

    private Rigidbody rb;

    private Vector3 postojumpto;

    private bool lookatplayer;

    private float enemyspeed;

    private float jumpspeed;

    private float slashrate;

    private float nextslash;

    private float nextalmostdeadcolor;

    private float almostdeadcolorrate;

    private float shurikenthrowrate;

    private float nextshuriken;

    [HideInInspector] public bool kicked;

    [HideInInspector] public bool slashing;

    [HideInInspector] public bool jumping;

    [HideInInspector] public bool posset;

    public float slashrange;

    public LayerMask whatcanbeslashed;

    public GameObject enemysprite;

    public GameObject shadow;

    public GameObject shuriken;

    public Transform shurikenspawnpos;

    public GameObject kickparticles;

    public float speed;

    public bool forplatforming;

    private float health;

    private void Start()
    {
        startattacking = false;
        Invoke("startatt", 1f);
        anim = enemysprite.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        lookatplayer = true;
        jumping = false;
        slashing = false;
        posset = false;
        nextslash = 0f;
        slashrate = 1.23f;
        nextshuriken = 0f;
        shurikenthrowrate = 0.78f;
        nextalmostdeadcolor = 0f;
        almostdeadcolorrate = 0.14f;
        jumpspeed = 14.56f;
        anim.SetBool("jumpend", false);
        anim.SetBool("getup", false);
        matcolor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        damagedcolor = new Color(Color.red.r, Color.red.g, Color.red.b, 1f);
        almostdeadcolor = new Color(1f, 0f, 0f, 1f);
        health = 3f;
    }

    private void Update()
    {
        if (health <= 0f)
        {
            Instantiate(kickparticles, transform.position, Quaternion.identity).transform.forward = transform.forward;
            Destroy(gameObject, 0f);
        }

        player = GameObject.FindGameObjectWithTag("Player");
        var worldPosition =
            new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        if (lookatplayer)
        {
            transform.LookAt(worldPosition);
        }

        anim.SetBool("getup", !kicked);
        if (health <= 1f && Time.time > nextalmostdeadcolor)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color =
                ((transform.GetChild(0).GetComponent<SpriteRenderer>().color == almostdeadcolor)
                    ? matcolor
                    : almostdeadcolor);
            nextalmostdeadcolor = Time.time + almostdeadcolorrate;
        }

        var num = Vector3.Distance(transform.position, player.transform.position);
        if (!startattacking)
        {
            return;
        }

        if (forplatforming)
        {
            anim.SetBool("moving", false);
            if (Time.time > nextshuriken && num >= 1.2f && !kicked)
            {
                anim.SetTrigger("throw");
                nextshuriken = Time.time + shurikenthrowrate;
            }

            return;
        }

        if (jumping)
        {
            if (posset)
            {
                if (Vector3.Distance(transform.position, postojumpto) > 1.5f && !anim.GetBool("jumpend"))
                {
                    var maxDistanceDelta = Time.deltaTime * jumpspeed;
                    transform.position = Vector3.MoveTowards(transform.position, postojumpto, maxDistanceDelta);
                    return;
                }

                anim.SetBool("jumpend", true);
            }

            return;
        }

        if (num <= 1.1f && !kicked && !slashing)
        {
            anim.SetBool("moving", false);
            if (Time.time > nextslash)
            {
                anim.SetTrigger("slash");
                nextslash = Time.time + slashrate;
            }
        }

        if (num > 1.1f && num < 5.6f && !jumping && !kicked && !slashing)
        {
            var maxDistanceDelta2 = Time.deltaTime * speed;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, maxDistanceDelta2);
            anim.SetBool("moving", true);
            return;
        }

        if (num >= 5.6f && num <= 7f && player.transform.position.y < transform.position.y + 2.5f && !jumping &&
            !slashing && !kicked)
        {
            anim.SetBool("moving", false);
            if (!jumping)
            {
                anim.SetTrigger("jump");
            }
        }
        else if (num >= 7f && !jumping && !slashing && !kicked)
        {
            anim.SetBool("moving", false);
            if (Time.time > nextshuriken)
            {
                anim.SetTrigger("throw");
                nextshuriken = Time.time + shurikenthrowrate;
            }
        }
    }

    private void startatt()
    {
        startattacking = true;
    }

    public void enemykickedback(float force)
    {
        health -= 0.6f;
        anim.SetTrigger("kicked");
        anim.SetBool("jumpend", true);
        if (kicked)
        {
            rb.AddForce(transform.GetChild(0).TransformDirection(0f, 0.75f, -1f) * force);
        }
        else
        {
            rb.AddForce(transform.GetChild(0).TransformDirection(0f, 0.68f, -1f) * force);
        }

        slashing = false;
        jumping = false;
        takendamage();
        kicked = true;
        shadow.SetActive(false);
        gameObject.layer = LayerMask.NameToLayer("Kicked Enemy");
    }

    public void enemykickedend()
    {
        kicked = false;
        if (!shadow.activeSelf)
        {
            shadow.SetActive(true);
        }

        gameObject.layer = LayerMask.NameToLayer("Enemy jumping");
    }

    public void jumpsetpos()
    {
        var vector = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        postojumpto = vector;
        posset = true;
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

    public void startslash()
    {
        slashing = true;
    }

    public void slash()
    {
        var array = Physics.OverlapSphere(shurikenspawnpos.position, slashrange, whatcanbeslashed);
        if (array.Length != 0)
        {
            var array2 = array;
            for (var i = 0; i < array2.Length; i++)
            {
                var collider = array2[i];
                if (collider.gameObject.tag == "Player")
                {
                    collider.gameObject.GetComponent<PlayerController>().takendamage();
                }
            }
        }
    }

    public void endslash()
    {
        slashing = false;
    }

    public void shurikenthrow()
    {
        Instantiate(shuriken, shurikenspawnpos.position, Quaternion.identity).GetComponent<shurikenscript>().spawnedby =
            gameObject.transform.GetChild(0).gameObject;
    }

    private void OnDrawGizmosSelected()
    {
        if (shurikenspawnpos == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(shurikenspawnpos.position, slashrange);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Kicked Enemy"))
        {
            takendamage();
            enemykickedback(19300f);
            if (col.gameObject.tag == "Floating enemy")
            {
                Instantiate(kickparticles, col.transform.position, Quaternion.identity).transform.forward =
                    col.transform.forward;
                Destroy(col.gameObject, 0f);
            }
        }

        if (col.gameObject.tag == "ground")
        {
            enemykickedend();
        }

        if (col.gameObject.tag == "arrow")
        {
            takendamage();
            if (col.gameObject.GetComponent<arrowscript>().cancausegrapple &&
                !transform.GetChild(0).GetComponent<greentargetscript>().arrowstate)
            {
                gameObject.transform.GetChild(0).GetComponent<greentargetscript>().SetArrowstate();
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enemytodashto =
                    gameObject.transform.GetChild(0).gameObject;
            }
        }

        if (col.gameObject.tag == "shuriken")
        {
            enemykickedback(16700f);
        }
    }
}