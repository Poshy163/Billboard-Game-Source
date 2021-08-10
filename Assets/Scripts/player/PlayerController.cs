#region

using Enemy;
using Other;
using Spawners;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 649

#endregion

// ReSharper disable All
#pragma warning disable 414
namespace player
{
    public class PlayerController:MonoBehaviour
    {
        public static Slider SlowTimer;
        public static GameObject Endlv;
        public static bool PlayerMove = true;
        public GameObject damagedpanel;
        public LayerMask whatcanbekicked;
        public float movespeed;
        public float dodgespeed;
        public float chargespeed;
        public float mousesensitivity;
        public float kickrange;
        public Camera viewcam;
        public GameObject Gun;
        public GameObject arrow;
        public GameObject grapplearrow;
        public GameObject Kick;
        public GameObject vaultkick;
        public GameObject dashscreen;
        public GameObject slowmoscreen;
        public Transform arrowspawnpos;
        [HideInInspector] public Vector3 movehorizontal;
        [HideInInspector] public Vector3 moveVertical;
        [HideInInspector] public bool shooting = false;
        [HideInInspector] public GameObject enemytodashto = null;
        public float slowdownlength = 2f;
        [HideInInspector] public int health;
        public float minangleofrotation;
        public float maxangleofrotation;
        public GameObject greencrosshair;
        public GameObject[] hearts;
        public Material[] Skyboxes;
        public GameObject[] Dashes;
        private bool charging = false;
        private float dodgerate = 0.306f;
        private bool dodging = false;
        private GameObject DText;
        private bool grounded = true;
        private Animator gunanim;
        private float horizontal;
        private bool inslowmo = false;
        private Animator kickanim;
        private bool longclick = false;
        private float mouseclickstart = 0f;
        private Vector3 mouseinput;
        private Vector3 moveinput;
        private float nextdodge = 0f;
        private Vector3 origscale;
        private Rigidbody rb;
        private bool shortclick = false;
        private readonly float slowdownfactor = 0.05f;
        private float vertical;


        private void Awake ()
        {
            SetSkyBox();
        }

        private void Start ()
        {
            if(GlobalVar.Name != null)
            {
                GlobalVar.CheckStats();
            }

            try
            {
                GameObject.Find("Timer").SetActive(true);
                AssignVar();
            }
            catch(Exception e)
            {
                DText.GetComponent<Text>().text = e.Message;
                DText.SetActive(true);
            }
        }

        public void Update ()
        {
            if(PlayerMove)
            {
                horizontal = horizontalaxis();
                vertical = verticalaxis();
                moveinput = new Vector3(horizontal,0f,vertical).normalized;
                movehorizontal = transform.right * moveinput.x;
                moveVertical = transform.forward * moveinput.z;
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                Endlv.SetActive(true);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if(Input.GetKeyDown(KeyCode.T) && SceneManager.GetActiveScene().name != "Lobby")
            {
                Endlv.SetActive(true);
                SceneManager.LoadScene("Lobby");
            }
            else if(Input.GetKeyDown(KeyCode.T) && SceneManager.GetActiveScene().name == "Lobby")
            {
                SceneManager.LoadScene("Main Menu");
            }


            #region Health

            try
            {
                for(int i = 0;i < health;i++)
                {
                    hearts[i].SetActive(true);
                }

                for(int j = health;j < 5;j++)
                {
                    hearts[j].SetActive(false);
                }

                if(health == 0)
                {
                    doslowmotion();
                    DText.SetActive(true);
                    Gun.SetActive(false);
                    damagedpanel.SetActive(true);
                    return;
                }
            }
            catch
            { }

            #endregion

            #region slow mode

            slowmoscreen.SetActive(inslowmo);
            if(Input.GetMouseButtonDown(1) && SlowTimer.value > 0.5f && PlayerMove ||
                Input.GetKeyDown(KeyCode.E) && SlowTimer.value > 0.5f && PlayerMove)
            {
                charging = false;
                dodging = false;
                inslowmo = !inslowmo;
                dodgerate -= 3f;
            }

            if(SlowTimer.value < 0.25f)
            {
                SlowTimer.value = 0.30f;
                charging = false;
                dodging = false;
                inslowmo = !inslowmo;
                dodgerate -= 3f;
            }
            else if(SlowTimer.value >= 100)
            {
                SlowTimer.gameObject.SetActive(false);
            }
            else
            {
                SlowTimer.gameObject.SetActive(true);
            }


            if(!inslowmo)
            {
                resettimescale();
                RegenSlider();
            }
            else if(inslowmo)
            {
                doslowmotion();
                DrainSlowMo();
            }

            void DrainSlowMo ()
            {
                SlowTimer.value -= (Time.deltaTime * GlobalVar.SlowModeDrainRate);
            }

            void RegenSlider ()
            {
                SlowTimer.value += (Time.deltaTime * GlobalVar.SlowModeRegenRate);
            }

            #endregion

            #region dashing

            if(charging && enemytodashto != null)
            {
                if(Vector3.Distance(transform.position,enemytodashto.transform.position) > 1.06f)
                {
                    gunanim.SetBool("charge",true);
                    float maxDistanceDelta = Time.deltaTime * chargespeed;
                    transform.position = Vector3.MoveTowards(transform.position,enemytodashto.transform.position,
                        maxDistanceDelta);
                    return;
                }

                rb.velocity = new Vector3(0f,0f,0f);
                soundmanagerscript.playsound("kick");
                rb.AddForce(transform.up * 3090f);
                kickstate(true,true);
                gunanim.SetBool("charge",false);
                charging = false;
                string tag = enemytodashto.tag;
                if(!(tag == "Fodder"))
                {
                    if(!(tag == "Gargoyle"))
                    {
                        if(!(tag == "Floating enemy"))
                        {
                            if(tag == "Summoner")
                            {
                                enemytodashto.GetComponent<greentargetscript>().SetArrowstate();
                                enemytodashto.GetComponent<Summonerscript>().kicked();
                            }
                        }
                        else
                        {
                            enemytodashto.GetComponent<Floatingenemyscript>()
                                .enemykickedback(viewcam.transform.forward);
                            enemytodashto.GetComponent<greentargetscript>().SetArrowstate();
                        }
                    }
                    else
                    {
                        enemytodashto.transform.parent.gameObject.GetComponent<gargoylescript>()
                            .enemykickedback(23000f);
                        enemytodashto.transform.parent.position = enemytodashto.transform.position;
                        enemytodashto.GetComponent<greentargetscript>().SetArrowstate();
                    }
                }

                enemytodashto = null;
            }
            else
            {
                gunanim.SetBool("charge",false);
                if(Input.GetKeyDown(KeyCode.LeftShift) && !Kick.activeSelf && !dodging && PlayerMove &&
                    (movehorizontal + moveVertical).magnitude != 0f && Time.time > nextdodge)
                {
                    if(inslowmo)
                    {
                        inslowmo = false;
                    }

                    nextdodge = Time.time + dodgerate;
                    gunanim.SetTrigger("dodge");
                    soundmanagerscript.playsound("dodge");
                }

                if(Input.GetMouseButtonDown(0) && PlayerMove)
                {
                    mouseclickstart = Time.time;
                }

                if(Input.GetMouseButton(0) && ((Time.time - mouseclickstart > 0.14 && !inslowmo) ||
                                                (Time.time - mouseclickstart > 0.02 && inslowmo)))
                {
                    longclick = true;
                }
                else if((Input.GetMouseButtonUp(0) && Time.time - mouseclickstart < 0.14) ||
                         (inslowmo && Input.GetMouseButtonUp(0)))
                {
                    shortclick = true;
                }
                else
                {
                    longclick = false;
                    shortclick = false;
                }

                if(shortclick && !shooting && !Kick.activeSelf && !dodging)
                {
                    if(enemytodashto == null)
                    {
                        gunanim.SetTrigger("shootinggreenarrow");
                    }
                    else
                    {
                        if(inslowmo)
                        {
                            inslowmo = false;
                        }

                        charging = true;
                        gunanim.SetTrigger("startcharge");
                    }

                    shortclick = false;
                }

                if(moveinput.magnitude != 0f && !dodging && PlayerMove)
                {
                    Vector3 vector = (movehorizontal + moveVertical) * movespeed;
                    rb.velocity = new Vector3(vector.x,rb.velocity.y,vector.z);
                    if(grounded)
                    {
                        gunanim.SetBool("moving",true);
                    }
                }
                else if(!dodging)
                {
                    rb.velocity = new Vector3(0f,rb.velocity.y,0f);
                    gunanim.SetBool("moving",false);
                }

                if(Input.GetKeyDown(KeyCode.Space) && grounded)
                {
                    grounded = false;
                    rb.AddForce(transform.up * 1300f);
                }

                //change to delta time if wanting to slow the camera with time
                mouseinput = new Vector3(Input.GetAxisRaw("Mouse Y"),-1f * Input.GetAxisRaw("Mouse X"),0f) *
                             mousesensitivity;
                float angle = viewcam.transform.rotation.eulerAngles.x - mouseinput.x;
                viewcam.transform.rotation = Quaternion.Euler(Clampangle(angle,minangleofrotation,maxangleofrotation),
                    viewcam.transform.rotation.eulerAngles.y,viewcam.transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y - mouseinput.y,transform.rotation.eulerAngles.z);
                if(enemytodashto == null)
                {
                    greencrosshair.SetActive(false);
                    return;
                }

                greencrosshair.SetActive(true);
            }

            #endregion
        }

        private void SetSkyBox ()
        {
            if(GlobalVar.Name == null)
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                return;
            }

            if(GlobalVar.Name.Contains("markey") || GlobalVar.Name.Contains("Markey"))
            {
                if(!(Camera.main is null))
                {
                    RenderSettings.skybox = Skyboxes[0];
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                }
            }
            else if(GlobalVar.Name.Contains("lewy") || GlobalVar.Name.Contains("Lewy"))
            {
                if(!(Camera.main is null))
                {
                    RenderSettings.skybox = Skyboxes[1];
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                }
            }
            else if(GlobalVar.Name.Contains("hayden") || GlobalVar.Name.Contains("Hayden"))
            {
                if(!(Camera.main is null))
                {
                    RenderSettings.skybox = Skyboxes[2];
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                }
            }
            else if(GlobalVar.Name.Contains("KEKW") || GlobalVar.Name.Contains("kekw"))
            {
                if(!(Camera.main is null))
                {
                    RenderSettings.skybox = Skyboxes[3];
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                }
            }
            else
            {
                if(!(Camera.main is null))
                {
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                }
                else
                {
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                }
            }
        }

        #region Movement

        private int verticalaxis ()
        {
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z))
            {
                return 1;
            }

            if(Input.GetKey(KeyCode.S))
            {
                return -1;
            }

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z))
            {
                Input.GetKey(KeyCode.S);
                return 0;
            }

            return 0;
        }

        private int horizontalaxis ()
        {
            if(Input.GetKey(KeyCode.D))
            {
                return 1;
            }

            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
            {
                return -1;
            }

            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
            {
                Input.GetKey(KeyCode.D);
                return 0;
            }

            return 0;
        }

        private float Clampangle ( float angle,float min,float max )
        {
            if(angle < 90f || angle > -90f)
            {
                if(angle > 180f)
                {
                    angle -= 360f;
                }

                if(max > 180f)
                {
                    max -= 360f;
                }

                if(min > 180f)
                {
                    min -= 360f;
                }
            }

            angle = Mathf.Clamp(angle,min,max);
            if(angle < 0f)
            {
                angle += 360f;
            }

            return angle;
        }

        #endregion

        #region Move and Shoot

        public void kickstate ( bool state,bool vault )
        {
            if(vault)
            {
                vaultkick.SetActive(state);
                Kick.SetActive(false);
            }
            else
            {
                Kick.SetActive(state);
                vaultkick.SetActive(false);
            }

            if(state)
            {
                Combo.AddToCombo();

                shootend();
                enddodge();
                gunanim.SetTrigger("kick");
                gunanim.SetBool("kicking",true);
                return;
            }

            gunanim.SetBool("kicking",false);
        }

        public void kick ()
        {
            dodging = false;
            viewcam.GetComponent<Camerascript>().camershake(65.56f);
            kickdamage();
        }

        public void shootstart ()
        {
            viewcam.GetComponent<Camerascript>().camershake(60.34f);
            shooting = true;
        }

        public void Shootarrow ()
        {
            Instantiate(arrow,arrowspawnpos.position,Quaternion.identity).transform.forward =
                viewcam.transform.forward;
        }

        public void Shootgrapplearrow ()
        {
            soundmanagerscript.playsound("arrowshoot");
            Instantiate(grapplearrow,arrowspawnpos.position,Quaternion.identity).transform.forward =
                viewcam.transform.forward;
        }


        public void shootend ()
        {
            shooting = false;
        }

        #endregion

        #region Dodge

        public void dodge ()
        {
            rb.useGravity = false;
            transform.localScale -= new Vector3(0f,0.1f,0f);
            dashscreen.SetActive(true);
            viewcam.fieldOfView = 60f;
            gunanim.SetBool("kicking",false);
            gunanim.SetBool("moving",false);
            dodging = true;
            shooting = false;
            if(inslowmo)
            {
                Vector3 vector = (movehorizontal + moveVertical) * (dodgespeed + 100f);
                rb.velocity = new Vector3(vector.x,0f,vector.z);
                return;
            }

            Vector3 vector2 = (movehorizontal + moveVertical) * dodgespeed;
            rb.velocity = new Vector3(vector2.x,0f,vector2.z);
        }

        public void enddodge ()
        {
            rb.useGravity = true;
            transform.localScale = origscale;
            dashscreen.SetActive(false);
            viewcam.fieldOfView = 60f;
            dodging = false;
        }

        #endregion

        #region Kicking

        public void kickdamage ()
        {
            Collider[] array = Physics.OverlapSphere(arrowspawnpos.position,kickrange,whatcanbekicked);
            if(array.Length != 0)
            {
                kickscript.kick(array,viewcam.transform.forward,transform.forward);
            }
        }

        public void takendamage ()
        {
            health--;
            damagedpanel.SetActive(true);
            Invoke("resetpanel",0.089f);
        }

        #endregion

        #region SlowMode Functions

        private void doslowmotion ()
        {
            enddodge();
            shootend();
            kickstate(false,false);
            gunanim.SetFloat("Setspeedofshooting",8.2f);
            kickanim.SetFloat("kickspeed",8.2f);
            Time.timeScale = slowdownfactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        private void resettimescale ()
        {
            gunanim.SetFloat("Setspeedofshooting",1f);
            kickanim.SetFloat("kickspeed",1f);
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        #endregion

        #region Other

        private void AssignVar ()
        {
            try
            {
                Endlv = GameObject.Find("Endlv");
                Endlv.SetActive(false);
                SlowTimer = GameObject.Find("TimeSlider").GetComponent<Slider>();
                health = 5;
                rb = GetComponent<Rigidbody>();
                gunanim = Gun.GetComponent<Animator>();
                kickanim = Kick.GetComponent<Animator>();
                greencrosshair = GameObject.Find("green crosshair");
                damagedpanel = GameObject.FindGameObjectWithTag("Damaged panel");
                damagedpanel.SetActive(false);
                slowmoscreen.SetActive(false);
                gunanim.SetBool("moving",false);
                gunanim.SetBool("kicking",false);
                gunanim.SetBool("charge",false);
                DText = GameObject.Find("DText");
                DText.SetActive(false);
                kickstate(false,false);
                dashscreen.SetActive(false);
                slowmoscreen.SetActive(false);
                origscale = gameObject.transform.localScale;
                Cursor.lockState = CursorLockMode.Locked;
                rb.useGravity = true;
                greencrosshair.SetActive(false);
                hearts[0] = GameObject.Find($"Image");
                for(int i = 1;i <= 4;i++)
                {
                    hearts[i] = GameObject.Find($"Image ({i})");
                }
            }
            catch(Exception ex)
            {
                DText.GetComponent<Text>().text = ex.Message;
                DText.SetActive(true);
                throw;
            }
        }


        private void resetpanel ()
        {
            damagedpanel.SetActive(false);
        }

        private void OnDrawGizmosSelected ()
        {
            if(arrowspawnpos == null)
            {
                return;
            }

            Gizmos.DrawWireSphere(arrowspawnpos.position,kickrange);
        }

        private void OnCollisionStay ( Collision col )
        {
            grounded = true;
        }

        private void OnCollisionExit ( Collision col )
        {
            if(col.gameObject.CompareTag("ground"))
            {
                AirTime.StartTime();
            }
        }

        private void OnCollisionEnter ( Collision col )
        {
            if(col.gameObject.CompareTag("ground"))
            {
                Combo.ResetCombo();
                AirTime.GetTime();
            }

            if(col.gameObject.CompareTag("TuTFloor"))
            {
                transform.position = new Vector3(0,-5.47f,-20.12f);
            }

            if(col.gameObject.CompareTag("shuriken"))
            {
                takendamage();
                soundmanagerscript.playsound("playerhurt");
            }

            if(col.gameObject.CompareTag("Floating enemy") &&
                !col.gameObject.GetComponent<Floatingenemyscript>().kicked &&
                !col.gameObject.GetComponent<greentargetscript>().arrowstate)
            {
                takendamage();
                Destroy(col.gameObject,0f);
            }

            if(col.gameObject.tag == "invisiblewall")
            {
                health = 0;
            }
        }

        private void OnTriggerEnter ( Collider other )
        {
            if(!other.CompareTag("shuriken"))
            {
                return;
            }

            Destroy(other.gameObject);
            health--;
        }

        #endregion
    }
}