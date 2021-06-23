#region

using System.Collections.Generic;
using Other;
using player;
using UnityEngine;

#endregion

// ReSharper disable All
//Formatted 
namespace Enemy
{
    public class Summonerscript : MonoBehaviour
    {
        public GameObject enemysprite;

        public float speed;

        public GameObject projectile;

        public GameObject floatinghead;

        public Transform projpos;

        public GameObject kickparticles;

        public GameObject basesummonerposition;

        private readonly float almostdeadcolorrate = 0.14f;

        private readonly float enemyspeed;

        private readonly bool lookatplayer = true;

        private readonly float shootrate = 1.63f;

        private Color almostdeadcolor = new Color(1f, 0f, 0f, 1f);

        private Animator anim;

        private bool corrupted = false;

        private GameObject currentpos;

        private Color damagedcolor = new Color(Color.red.r, Color.red.g, Color.red.b, 1f);

        private float health = 30f;

        private Color matcolor;

        private bool moving;

        private float nextalmostdeadcolor = 0f;

        private float nextshoot = 1f;
        private GameObject player;

        private Rigidbody rb;

        private bool shooting;

        private GameObject[] summonerpositions;

        private TrailRenderer trailrenderer;

        private Color transparentcolor;

        private Color Unkillablecolor = new Color(1, 0.9170542f, 0.006289184f, 1);

        public void Start()
        {
            anim = enemysprite.GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            trailrenderer = GetComponent<TrailRenderer>();
            matcolor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            transparentcolor = new Color(matcolor.r, matcolor.g, matcolor.b, 0.27f);
            summonerpositions = GameObject.FindGameObjectsWithTag("Summonerpos");
            transform.position = basesummonerposition.transform.position;
            currentpos = basesummonerposition;
            trailrenderer.enabled = false;
        }

        public void Update()
        {
            if (health <= 0f)
            {
                Instantiate(kickparticles, transform.position, Quaternion.identity).transform.forward =
                    transform.forward;
                GameObject[] array = GameObject.FindGameObjectsWithTag("Gargoyle");
                for (int i = 0; i < array.Length; i++)
                {
                    GameObject gameObject = array[i];
                    if (!(gameObject.name == "gargoyle sprite"))
                    {
                        gameObject.GetComponent<gargoylescript>().enemykickedback(5f);
                    }
                }

                PlayerController.SlowTimer.value += 5;
                Destroy(this.gameObject, 0f);
            }

            player = GameObject.FindGameObjectWithTag("Player");
            Vector3 worldPosition =
                new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            Vector3 worldPosition2 = new Vector3(player.transform.position.x, player.transform.position.y,
                player.transform.position.z);
            if (lookatplayer)
            {
                transform.LookAt(worldPosition);
                enemysprite.transform.LookAt(worldPosition2);
            }

            if (health <= 1f &&
                transform.GetChild(0).GetComponent<SpriteRenderer>().material.color != transparentcolor &&
                Time.time > nextalmostdeadcolor)
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().color =
                    ((transform.GetChild(0).GetComponent<SpriteRenderer>().color == almostdeadcolor)
                        ? matcolor
                        : almostdeadcolor);
                nextalmostdeadcolor = Time.time + almostdeadcolorrate;
            }

            Vector3.Distance(transform.position, player.transform.position);
            float num = Vector3.Distance(transform.position, currentpos.transform.position);
            if (Time.time > nextshoot && !shooting && num < 0.5f && !moving)
            {
                nextshoot = Time.time + shootrate;
                anim.SetTrigger("shoot");
                if (!corrupted)
                {
                    corrupted = true;
                    try
                    {
                        currentpos.GetComponent<summonerposscript>().corruptpillar();
                    }
                    catch
                    { }
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
                    float maxDistanceDelta = Time.deltaTime * speed;
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

        public void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Kicked Enemy"))
            {
                takendamage();
                kicked();
                if (col.gameObject.CompareTag("Floating enemy"))
                {
                    Instantiate(kickparticles, col.transform.position, Quaternion.identity).transform.forward =
                        col.transform.forward;
                    Destroy(col.gameObject, 0f);
                }
            }

            if (col.gameObject.CompareTag("arrow") &&
                transform.GetChild(0).GetComponent<SpriteRenderer>().color != transparentcolor)
            {
                takendamage();
                GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                if (col.gameObject.GetComponent<arrowscript>().cancausegrapple &&
                    !transform.GetComponent<greentargetscript>().arrowstate)
                {
                    gameObject.GetComponent<greentargetscript>().SetArrowstate();
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enemytodashto =
                        gameObject;
                }
            }
        }

        public void shoot()
        {
            if (Random.Range(0, 2) <= 1)
            {
                soundmanagerscript.playsound("enemyshoot");
                GameObject ball = Instantiate(projectile, transform.position, Quaternion.identity);
                ball.GetComponent<Rigidbody>()
                    .AddForce((transform.GetChild(0).transform.forward * GlobalVar.BulletSpeed * 100));
                ball.transform.localScale *= 3;
            }
        }

        public void shootstart()
        {
            shooting = true;
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
            Invoke(nameof(resetcolor), 0.098f);
        }

        public void Unkillable()
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Unkillablecolor;
        }


        private void resetcolor()
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = matcolor;
        }

        public void kicked()
        {
            Instantiate(kickparticles, transform.position, Quaternion.identity).transform.forward =
                enemysprite.transform.forward;
            health -= 5f;
            takendamage();
            shootend();
            anim.SetTrigger("moving");
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = transparentcolor;
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            List<GameObject> list = new List<GameObject>(summonerpositions);
            list.Remove(currentpos);
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].GetComponent<summonerposscript>().checkpillarstate())
                {
                    num++;
                }
            }

            if (num == list.Count)
            {
                System.Random random = new System.Random();
                currentpos = list[random.Next(0, list.Count - 1)];
                return;
            }

            List<GameObject> list2 = new List<GameObject>(list);
            for (int j = 0; j < list2.Count; j++)
            {
                if (!list2[j].GetComponent<summonerposscript>().checkpillarstate())
                {
                    list2.Remove(list2[j]);
                }

                System.Random random2 = new System.Random();
                currentpos = list2[random2.Next(0, list2.Count - 1)];
            }
        }
    }
}