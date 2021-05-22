using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
//Formatted
// ReSharper disable All
#pragma warning disable 414
public class gargoylescript : MonoBehaviour
{
    private GameObject player;

    private Color matcolor;

    private Color damagedcolor = new Color(Color.red.r,Color.red.g,Color.red.b,1f);

    private Color almostdeadcolor = new Color(1f,0f,0f,1f);

    private Animator anim;

    private Rigidbody rb;

    public GameObject enemysprite;

    public GameObject bullet;

    public GameObject kickparticles;

    public GameObject postorotatearound;

    private bool lookatplayer = true;

    public bool dontattack;

    [HideInInspector] public bool kicked = false;

    private float speed = 7f;

    private float nextshoot = 1f;

    private float shootrate = 1.76f;

    private float nextalmostdeadcolor = 0f;

    private float almostdeadcolorrate = 0.14f;

    private bool startattacking = true;

    private float health = 5f;

    private float rotationspeed = 1.6f;

    private float rotationspeedslowmo = 0.15f;

    private int dir;

    public void Start()
    {
        dir = ((Random.Range(0f,1f) > 0.5f) ? 1 : -1);
        anim = enemysprite.GetComponent<Animator>();
        anim.SetBool("kickend", true);
        rb = GetComponent<Rigidbody>();
        Invoke("startat", 1f);
        matcolor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
    }

    public void FixedUpdate()
    {
        if (health <= 0f)
        {
            if (postorotatearound != null)
            {
                postorotatearound.GetComponent<castlescript>().spawnedgargoyle.RemoveAt(0);
            }

            Instantiate(kickparticles, transform.position, Quaternion.identity).transform.forward = transform.forward;
            PlayerController.SlowTimer.value += 5;
            Destroy(gameObject, 0f);
        }

        player = GameObject.FindGameObjectWithTag("Player");
        var worldPosition =  new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        var worldPosition2 = new Vector3(player.transform.position.x, player.transform.position.y,
            player.transform.position.z);
        if (lookatplayer)
        {
            transform.LookAt(worldPosition);
            transform.GetChild(0).LookAt(worldPosition2);
        }

        if (health <= 1f && Time.time > nextalmostdeadcolor)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color =
                ((transform.GetChild(0).GetComponent<SpriteRenderer>().color == almostdeadcolor)
                    ? matcolor
                    : almostdeadcolor);
            nextalmostdeadcolor = Time.time + almostdeadcolorrate;
        }

        if (!player.activeSelf)
        {
            return;
        }

        if (!kicked && Time.time > nextshoot && !dontattack)
        {
            anim.SetTrigger("shoot");
            nextshoot = Time.time + shootrate;
        }

        if (Time.timeScale >= 1f && postorotatearound != null)
        {
            transform.RotateAround(postorotatearound.transform.position, Vector3.up, rotationspeed * dir);
            return;
        }

        if (postorotatearound != null)
        {
            transform.RotateAround(postorotatearound.transform.position, Vector3.up, rotationspeedslowmo * dir);
        }
    }
    public void takendamage()
    {
        if (health > 1f)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = damagedcolor;
            Invoke("resetcolor", 0.25f);
        }
    }
    public void resetcolor ()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = matcolor;
    }
    public void startat ()
    {
        startattacking = true;
    }
    public void shoot ()
    {
        soundmanagerscript.playsound("enemyshoot");
        //Instantiate(bullet,transform.position,Quaternion.identity).GetComponent<shurikenscript>().spawnedby =
            //gameObject.transform.GetChild(0).gameObject;
    }
    public void enemykickedback(float force)
    {
        Instantiate(kickparticles, transform.position, Quaternion.identity).transform.forward =
            enemysprite.transform.forward;
        health -= 5f;
        nextshoot += 1f;
        kicked = true;
        takendamage();
        Invoke("endkick", 0.67f);
        anim.SetBool("kickend", false);
        anim.SetTrigger("kicked");
        rb.AddForce(transform.forward * -1f * force);
        gameObject.layer = LayerMask.NameToLayer("Kicked Enemy");
    }
    public void endkick()
    {
        rb.velocity = new Vector3(0f, 0f, 0f);
        kicked = false;
        anim.SetBool("kickend", true);
        gameObject.layer = LayerMask.NameToLayer("Enemy jumping");
    }
    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Kicked Enemy"))
        {
            takendamage();
            enemykickedback(16900f);
            if (col.gameObject.tag == "Floating enemy")
            {
                Instantiate(kickparticles, col.transform.position, Quaternion.identity).transform.forward =
                    col.transform.forward;
                Debug.Log("Kicked Enemy");
                Destroy(col.gameObject, 0f);
            }
        }

        if (col.gameObject.tag == "arrow" && !col.gameObject.GetComponent<arrowscript>().hit)
        {
            takendamage();
            col.gameObject.GetComponent<arrowscript>().hit = true;
            if (col.gameObject.GetComponent<arrowscript>().cancausegrapple &&
                !transform.GetChild(0).GetComponent<greentargetscript>().arrowstate)
            {
                var list = new List<GameObject>(GameObject.FindGameObjectsWithTag("Gargoyle"));
                list.Remove(gameObject);
                if (list.Count > 0)
                {
                    foreach (var current in list)
                    {
                        if (!(current.name == "gargoyle sprite") && current.transform.GetChild(0)
                            .GetComponent<greentargetscript>().arrowstate)
                        {
                            current.transform.GetChild(0).GetComponent<greentargetscript>().SetArrowstate();
                        }
                    }
                }

                gameObject.transform.GetChild(0).GetComponent<greentargetscript>().SetArrowstate();
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enemytodashto =
                    gameObject.transform.GetChild(0).gameObject;
            }
        }

        if (col.gameObject.tag == "Gargoyle")
        {
            dir *= -1;
        }
    }
}