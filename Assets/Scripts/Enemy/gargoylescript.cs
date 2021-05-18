using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
#pragma warning disable 414

// ReSharper disable All
public class gargoylescript : MonoBehaviour
{
    private GameObject player;

    private Color matcolor;

    private Color damagedcolor;

    private Color almostdeadcolor;

    private Animator anim;

    private Rigidbody rb;

    public GameObject enemysprite;

    public GameObject bullet;

    public GameObject kickparticles;

    public GameObject postorotatearound;

    private bool lookatplayer;

    public bool dontattack;

    [HideInInspector] public bool kicked;

    private float speed;

    private float nextshoot;

    private float shootrate;

    private float nextalmostdeadcolor;

    private float almostdeadcolorrate;

    private bool startattacking;

    private float health;

    private float rotationspeed;

    private float rotationspeedslowmo;

    private int dir;

    private void Start()
    {
        rotationspeed = 0.7f;
        rotationspeedslowmo = 0.09f;
        dir = ((Random.Range(0f, 1f) > 0.5f) ? 1 : -1);
        speed = 3f;
        nextshoot = 1f;
        shootrate = 1.76f;
        nextalmostdeadcolor = 0f;
        almostdeadcolorrate = 0.14f;
        kicked = false;
        anim = enemysprite.GetComponent<Animator>();
        anim.SetBool("kickend", true);
        rb = GetComponent<Rigidbody>();
        Invoke("startat", 1f);
        startattacking = false;
        lookatplayer = true;
        matcolor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        damagedcolor = new Color(Color.red.r, Color.red.g, Color.red.b, 1f);
        almostdeadcolor = new Color(1f, 0f, 0f, 1f);
        health = 5f;
    }

    private void Update()
    {
        if (health <= 0f)
        {
            if (postorotatearound != null)
            {
                postorotatearound.GetComponent<castlescript>().spawnedgargoyle.RemoveAt(0);
            }

            Instantiate(kickparticles, transform.position, Quaternion.identity).transform.forward = transform.forward;
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

    private void resetcolor()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = matcolor;
    }

    private void startat()
    {
        startattacking = true;
    }

    public void shoot()
    {
        soundmanagerscript.playsound("enemyshoot");
        Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<shurikenscript>().spawnedby =
            gameObject.transform.GetChild(0).gameObject;
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

    private void endkick()
    {
        rb.velocity = new Vector3(0f, 0f, 0f);
        kicked = false;
        anim.SetBool("kickend", true);
        gameObject.layer = LayerMask.NameToLayer("Enemy jumping");
    }

    private void OnCollisionEnter(Collision col)
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