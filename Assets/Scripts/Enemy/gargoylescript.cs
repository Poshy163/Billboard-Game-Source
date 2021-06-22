#region

using Other;
using player;
using Spawners;
using System.Collections.Generic;
using UnityEngine;

#endregion

//Formatted
// ReSharper disable All
#pragma warning disable 414
namespace Enemy
{
    public class gargoylescript:MonoBehaviour
    {
        public GameObject enemysprite;

        public GameObject bullet;

        public GameObject kickparticles;

        public GameObject postorotatearound;

        public bool dontattack;

        [HideInInspector] public bool kicked = false;

        private readonly float almostdeadcolorrate = 0.14f;

        private readonly bool lookatplayer = true;
        private readonly GameObject PlayerTarget;

        private readonly float rotationspeed = 1.6f;

        private readonly float rotationspeedslowmo = 0.15f;

        private readonly float shootrate = 1.76f;

        private readonly float speed = 7f;

        private Color almostdeadcolor = new Color(1f,0f,0f,1f);

        private Animator anim;

        private Color damagedcolor = new Color(Color.red.r,Color.red.g,Color.red.b,1f);

        private int dir;

        private float health = 5f;

        private Color matcolor;

        private float nextalmostdeadcolor = 0f;

        private float nextshoot = 1f;
        private GameObject player;

        private Rigidbody rb;

        private bool startattacking = true;

        public void Start ()
        {
            if(!dontattack)
            {
                dontattack = GlobalVar.Enemydontattack;
            }

            dir = ((Random.Range(0f,1f) > 0.5f) ? 1 : -1);
            anim = enemysprite.GetComponent<Animator>();
            anim.SetBool("kickend",true);
            rb = GetComponent<Rigidbody>();
            Invoke("startat",1f);
            matcolor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            player = GameObject.FindGameObjectWithTag("Player");
        }

        public void FixedUpdate ()
        {
            if(health <= 0f)
            {
                if(postorotatearound != null)
                {
                    postorotatearound.GetComponent<castlescript>().spawnedgargoyle.RemoveAt(0);
                }

                Instantiate(kickparticles,transform.position,Quaternion.identity).transform.forward =
                    transform.forward;
                PlayerController.SlowTimer.value += 5;
                Destroy(gameObject,0f);
            }

            Vector3 worldPosition =
                new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z);
            Vector3 worldPosition2 = new Vector3(player.transform.position.x,player.transform.position.y,
                player.transform.position.z);
            if(lookatplayer)
            {
                transform.LookAt(worldPosition);
                transform.GetChild(0).LookAt(worldPosition2);
            }

            if(health <= 1f && Time.time > nextalmostdeadcolor)
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().color =
                    ((transform.GetChild(0).GetComponent<SpriteRenderer>().color == almostdeadcolor)
                        ? matcolor
                        : almostdeadcolor);
                nextalmostdeadcolor = Time.time + almostdeadcolorrate;
            }

            if(!player.activeSelf)
            {
                return;
            }

            if(!kicked && Time.time > nextshoot && !dontattack)
            {
                anim.SetTrigger("shoot");
                nextshoot = Time.time + shootrate;
            }

            if(Time.timeScale >= 1f && postorotatearound != null)
            {
                transform.RotateAround(postorotatearound.transform.position,Vector3.up,rotationspeed * dir);
                return;
            }

            if(postorotatearound != null)
            {
                transform.RotateAround(postorotatearound.transform.position,Vector3.up,rotationspeedslowmo * dir);
            }
        }

        public void OnCollisionEnter ( Collision col )
        {
            if(col.gameObject.layer == LayerMask.NameToLayer("Kicked Enemy"))
            {
                takendamage();
                enemykickedback(16900f);
                if(col.gameObject.CompareTag("Floating enemy"))
                {
                    Instantiate(kickparticles,col.transform.position,Quaternion.identity).transform.forward =
                        col.transform.forward;
                    Debug.Log("Kicked Enemy");
                    Destroy(col.gameObject,0f);
                }
            }

            if(col.gameObject.CompareTag("arrow") && !col.gameObject.GetComponent<arrowscript>().hit)
            {
                takendamage();
                col.gameObject.GetComponent<arrowscript>().hit = true;
                if(col.gameObject.GetComponent<arrowscript>().cancausegrapple &&
                    !transform.GetChild(0).GetComponent<greentargetscript>().arrowstate)
                {
                    List<GameObject> list = new List<GameObject>(GameObject.FindGameObjectsWithTag("Gargoyle"));
                    list.Remove(gameObject);
                    if(list.Count > 0)
                    {
                        foreach(GameObject current in list)
                        {
                            if(!(current.name == "gargoyle sprite") && current.transform.GetChild(0)
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

            if(col.gameObject.tag == "Gargoyle")
            {
                dir *= -1;
            }
        }

        public void takendamage ()
        {
            if(health > 1f)
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = damagedcolor;
                Invoke("resetcolor",0.25f);
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
            if(Random.Range(1,6) >= GlobalVar.ShootChance)
            {
                soundmanagerscript.playsound("enemyshoot");
                Instantiate(bullet,transform.position,Quaternion.identity).GetComponent<Rigidbody>()
                    .AddForce((transform.GetChild(0).transform.forward * GlobalVar.BulletSpeed * 100));
            }
        }

        public void enemykickedback ( float force )
        {
            Instantiate(kickparticles,transform.position,Quaternion.identity).transform.forward =
                enemysprite.transform.forward;
            health -= 5f;
            nextshoot += 1f;
            kicked = true;
            takendamage();
            Invoke("endkick",0.67f);
            anim.SetBool("kickend",false);
            anim.SetTrigger("kicked");
            rb.AddForce(transform.forward * -1f * force);
            gameObject.layer = LayerMask.NameToLayer("Kicked Enemy");
        }

        public void endkick ()
        {
            rb.velocity = new Vector3(0f,0f,0f);
            kicked = false;
            anim.SetBool("kickend",true);
            gameObject.layer = LayerMask.NameToLayer("Enemy jumping");
        }
    }
}