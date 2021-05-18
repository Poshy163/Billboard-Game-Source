using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
// ReSharper disable All
public class Summonerscript : MonoBehaviour
{
    private GameObject player;

    private Color matcolor;

    private Color transparentcolor;

    private Color damagedcolor;

    private Color almostdeadcolor;

    private Animator anim;

    private Rigidbody rb;

    private GameObject[] summonerpositions;

    private GameObject currentpos;

    private TrailRenderer trailrenderer;

    private bool lookatplayer;

    private bool shooting;

    private bool moving;

    private float enemyspeed;

    private float nextshoot;

    private float shootrate;

    private float health;

    private float nextalmostdeadcolor;

    private float almostdeadcolorrate;

    public GameObject enemysprite;

    public GameObject finaltext;

    public float speed;

    public GameObject projectile;

    public GameObject floatinghead;

    public Transform projpos;

    public GameObject kickparticles;

    public GameObject basesummonerposition;

    private bool corrupted;

    private void Start()
    {
        anim = enemysprite.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        trailrenderer = GetComponent<TrailRenderer>();
        lookatplayer = true;
        shooting = false;
        nextshoot = 1f;
        shootrate = 1.63f;
        health = 8f;
        nextalmostdeadcolor = 0f;
        almostdeadcolorrate = 0.14f;
        matcolor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        transparentcolor = new Color(matcolor.r, matcolor.g, matcolor.b, 0.27f);
        damagedcolor = new Color(Color.red.r, Color.red.g, Color.red.b, 1f);
        almostdeadcolor = new Color(1f, 0f, 0f, 1f);
        summonerpositions = GameObject.FindGameObjectsWithTag("Summonerpos");
        transform.position = basesummonerposition.transform.position;
        currentpos = basesummonerposition;
        trailrenderer.enabled = false;
        corrupted = false;
        finaltext.SetActive(false);
    }

    private void Update()
    {
        if (health <= 0f)
        {
            finaltext.SetActive(true);
            Instantiate(kickparticles, transform.position, Quaternion.identity).transform.forward = transform.forward;
            var array = GameObject.FindGameObjectsWithTag("Gargoyle");
            for (var i = 0; i < array.Length; i++)
            {
                var gameObject = array[i];
                if (!(gameObject.name == "gargoyle sprite"))
                {
                    gameObject.GetComponent<gargoylescript>().enemykickedback(5f);
                }
            }

            Destroy(this.gameObject, 0f);
        }

        player = GameObject.FindGameObjectWithTag("Player");
        var worldPosition =
            new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        var worldPosition2 = new Vector3(player.transform.position.x, player.transform.position.y,
            player.transform.position.z);
        if (lookatplayer)
        {
            transform.LookAt(worldPosition);
            enemysprite.transform.LookAt(worldPosition2);
        }

        if (health <= 1f && transform.GetChild(0).GetComponent<SpriteRenderer>().material.color != transparentcolor &&
            Time.time > nextalmostdeadcolor)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color =
                ((transform.GetChild(0).GetComponent<SpriteRenderer>().color == almostdeadcolor)
                    ? matcolor
                    : almostdeadcolor);
            nextalmostdeadcolor = Time.time + almostdeadcolorrate;
        }

        Vector3.Distance(transform.position, player.transform.position);
        var num = Vector3.Distance(transform.position, currentpos.transform.position);
        if (Time.time > nextshoot && !shooting && num < 0.5f && !moving)
        {
            nextshoot = Time.time + shootrate;
            anim.SetTrigger("shoot");
            if (!corrupted)
            {
                corrupted = true;
                currentpos.GetComponent<summonerposscript>().corruptpillar();
            }
        }
        else
        {
            if (num >= 0.5f)
            {
                corrupted = false;
                trailrenderer.enabled = true;
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = transparentcolor;
                gameObject.layer = LayerMask.NameToLayer("Summonermoving");
                moving = true;
                var maxDistanceDelta = Time.deltaTime * speed;
                transform.position =
                    Vector3.MoveTowards(transform.position, currentpos.transform.position, maxDistanceDelta);
                return;
            }

            gameObject.layer = LayerMask.NameToLayer("Enemy");
            moving = false;
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = matcolor;
            trailrenderer.enabled = false;
        }
    }

    public void shootstart()
    {
        shooting = true;
    }

    public void shoot()
    {
        soundmanagerscript.playsound("enemyshoot");
        Instantiate(projectile, projpos.position, Quaternion.identity).GetComponent<shurikenscript>().spawnedby =
            gameObject.transform.GetChild(0).gameObject;
    }

    public void spawnhead()
    {
        Instantiate(floatinghead, projpos.position, Quaternion.identity);
    }

    public void shootend()
    {
        shooting = false;
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

    public void kicked()
    {
        Instantiate(kickparticles, transform.position, Quaternion.identity).transform.forward =
            enemysprite.transform.forward;
        health -= 1f;
        Debug.Log(health);
        takendamage();
        shootend();
        anim.SetTrigger("moving");
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = transparentcolor;
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        var list = new List<GameObject>(summonerpositions);
        list.Remove(currentpos);
        var num = 0;
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].GetComponent<summonerposscript>().checkpillarstate())
            {
                num++;
            }
        }

        if (num == list.Count)
        {
            var random = new Random();
            currentpos = list[random.Next(0, list.Count - 1)];
            return;
        }

        var list2 = new List<GameObject>(list);
        for (var j = 0; j < list2.Count; j++)
        {
            if (!list2[j].GetComponent<summonerposscript>().checkpillarstate())
            {
                list2.Remove(list2[j]);
            }

            var random2 = new Random();
            currentpos = list2[random2.Next(0, list2.Count - 1)];
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Kicked Enemy"))
        {
            takendamage();
            kicked();
            if (col.gameObject.tag == "Floating enemy")
            {
                Instantiate(kickparticles, col.transform.position, Quaternion.identity).transform.forward =
                    col.transform.forward;
                Destroy(col.gameObject, 0f);
            }
        }

        if (col.gameObject.tag == "arrow" &&
            transform.GetChild(0).GetComponent<SpriteRenderer>().color != transparentcolor)
        {
            takendamage();
            GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            if (col.gameObject.GetComponent<arrowscript>().cancausegrapple &&
                !transform.GetComponent<greentargetscript>().arrowstate)
            {
                gameObject.GetComponent<greentargetscript>().SetArrowstate();
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enemytodashto = gameObject;
            }
        }

        if (col.gameObject.tag == "shuriken")
        {
            takendamage();
            kicked();
        }
    }
}