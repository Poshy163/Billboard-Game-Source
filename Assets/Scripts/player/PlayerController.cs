using TMPro;
using UnityEngine;
// ReSharper disable All
#pragma warning disable 414
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 moveinput;

    private Vector3 mouseinput;

    private Animator gunanim;

    private Animator kickanim;

    private Vector3 origscale;

    private GameObject damagedpanel;

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

    [HideInInspector] public bool shooting;

    [HideInInspector] public GameObject enemytodashto;

    private float horizontal;

    private float vertical;

    private GameObject DText;
    
    private float nextkick;

    private float kickrate;

    private float nextdodge;

    private float dodgerate;

    private bool dodging;

    private bool grounded;

    private bool charging;

    private bool longclick;

    private bool shortclick;

    private float mouseclickstart;

    public float slowdownfactor = 0.05f;

    public float slowdownlength = 2f;

    private bool inslowmo;

    [HideInInspector] public int health;

    public float minangleofrotation;

    public float maxangleofrotation;

    public GameObject greencrosshair;

    public GameObject[] hearts;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gunanim = Gun.GetComponent<Animator>();
        kickanim = Kick.GetComponent<Animator>();
        greencrosshair = GameObject.Find("green crosshair");
        damagedpanel = GameObject.FindGameObjectWithTag("Damaged panel");
        damagedpanel.SetActive(false);
        slowmoscreen.SetActive(false);
        gunanim.SetBool("moving", false);
        gunanim.SetBool("kicking", false);
        gunanim.SetBool("charge", false);
        shooting = false;
        DText = GameObject.Find("DText");
        DText.SetActive(false);
        dodging = false;
        charging = false;
        kickstate(false, false);
        dashscreen.SetActive(false);
        slowmoscreen.SetActive(false);
        nextkick = 0f;
        kickrate = 0.34f;
        nextdodge = 0f;
        dodgerate = 0.306f;
        grounded = true;
        origscale = gameObject.transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        rb.useGravity = true;
        mouseclickstart = 0f;
        longclick = false;
        shortclick = false;
        inslowmo = false;
        enemytodashto = null;
        health = 5;
        greencrosshair.SetActive(false);
        hearts[0] = GameObject.Find($"Image");

        for(var i = 1;i <= 4; i++)
        {
            hearts[i] = GameObject.Find($"Image ({i})");
        }
    }

    private void Update()
    {
        horizontal = horizontalaxis();
        vertical = verticalaxis();
        moveinput = new Vector3(horizontal, 0f, vertical).normalized;
        movehorizontal = transform.right * moveinput.x;
        moveVertical = transform.forward * moveinput.z;
        for (var i = 0; i < health; i++)
        {
            hearts[i].SetActive(true);
        }

        for (var j = health - 1; j < 5; j++)
        {
            hearts[j].SetActive(false);
        }

        if (health == 1)
        {
            DText.SetActive(true);
            Gun.SetActive(false);
            damagedpanel.SetActive(true);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Application.Quit();
        }

        slowmoscreen.SetActive(inslowmo);
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
        {
            charging = false;
            dodging = false;
            inslowmo = !inslowmo;
            dodgerate -= 3f;
        }

        if (!inslowmo)
        {
            resettimescale();
        }
        else if (inslowmo)
        {
            doslowmotion();
        }

        if (charging && enemytodashto != null)
        {
            if (Vector3.Distance(transform.position, enemytodashto.transform.position) > 1.06f)
            {
                gunanim.SetBool("charge", true);
                var maxDistanceDelta = Time.deltaTime * chargespeed;
                transform.position = Vector3.MoveTowards(transform.position, enemytodashto.transform.position,
                    maxDistanceDelta);
                return;
            }

            rb.velocity = new Vector3(0f, 0f, 0f);
            soundmanagerscript.playsound("kick");
            rb.AddForce(transform.up * 3090f);
            kickstate(true, true);
            gunanim.SetBool("charge", false);
            charging = false;
            var tag = enemytodashto.tag;
            if (!(tag == "Fodder"))
            {
                if (!(tag == "Gargoyle"))
                {
                    if (!(tag == "Floating enemy"))
                    {
                        if (tag == "Summoner")
                        {
                            enemytodashto.GetComponent<greentargetscript>().SetArrowstate();
                            enemytodashto.GetComponent<Summonerscript>().kicked();
                        }
                    }
                    else
                    {
                        enemytodashto.GetComponent<Floatingenemyscript>().enemykickedback(viewcam.transform.forward);
                        enemytodashto.GetComponent<greentargetscript>().SetArrowstate();
                    }
                }
                else
                {
                    
                    enemytodashto.transform.parent.gameObject.GetComponent<gargoylescript>().enemykickedback(23000f);
                    enemytodashto.transform.parent.position = enemytodashto.transform.position;
                    enemytodashto.GetComponent<greentargetscript>().SetArrowstate();
                }
            }
            else
            {
                
                enemytodashto.transform.parent.gameObject.GetComponent<fodderenemyscript>().enemykickedback(32400f);
                enemytodashto.transform.parent.position = enemytodashto.transform.position;
                enemytodashto.GetComponent<greentargetscript>().SetArrowstate();
            }

            enemytodashto = null;
        }
        else
        {
            gunanim.SetBool("charge", false);
            if (Input.GetKeyDown(KeyCode.LeftShift) && !Kick.activeSelf && !dodging &&
                (movehorizontal + moveVertical).magnitude != 0f && Time.time > nextdodge)
            {
                if (inslowmo)
                {
                    inslowmo = false;
                }

                nextdodge = Time.time + dodgerate;
                gunanim.SetTrigger("dodge");
                soundmanagerscript.playsound("dodge");
            }

            if (Input.GetMouseButtonDown(0))
            {
                mouseclickstart = Time.time;
            }

            if (Input.GetMouseButton(0) && ((Time.time - mouseclickstart > 0.14 && !inslowmo) ||
                                            (Time.time - mouseclickstart > 0.02 && inslowmo)))
            {
                longclick = true;
            }
            else if ((Input.GetMouseButtonUp(0) && Time.time - mouseclickstart < 0.14) ||
                     (inslowmo && Input.GetMouseButtonUp(0)))
            {
                shortclick = true;
            }
            else
            {
                longclick = false;
                shortclick = false;
            }

            if (shortclick && !longclick && !shooting && !Kick.activeSelf && !dodging)
            {
                if (enemytodashto == null)
                {
                    gunanim.SetTrigger("shootinggreenarrow");
                }
                else
                {
                    if (inslowmo)
                    {
                        inslowmo = false;
                    }

                    charging = true;
                    gunanim.SetTrigger("startcharge");
                }

                shortclick = false;
            }

            if (Input.GetKeyDown(KeyCode.F) && enemytodashto != null)
            {
                enemytodashto.GetComponent<greentargetscript>().SetArrowstate();
                enemytodashto = null;
            }

            if (moveinput.magnitude != 0f && !dodging)
            {
                var vector = (movehorizontal + moveVertical) * movespeed;
                rb.velocity = new Vector3(vector.x, rb.velocity.y, vector.z);
                if (grounded)
                {
                    gunanim.SetBool("moving", true);
                }
            }
            else if (!dodging)
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
                gunanim.SetBool("moving", false);
            }

            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                grounded = false;
                rb.AddForce(transform.up * 1300f);
            }

            mouseinput = new Vector3(Input.GetAxisRaw("Mouse Y"), -1f * Input.GetAxisRaw("Mouse X"), 0f) *
                         mousesensitivity;
            var angle = viewcam.transform.rotation.eulerAngles.x - mouseinput.x;
            viewcam.transform.rotation = Quaternion.Euler(Clampangle(angle, minangleofrotation, maxangleofrotation),
                viewcam.transform.rotation.eulerAngles.y, viewcam.transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y - mouseinput.y, transform.rotation.eulerAngles.z);
            if (enemytodashto == null)
            {
                greencrosshair.SetActive(false);
                return;
            }

            greencrosshair.SetActive(true);
        }
    }

    private int verticalaxis()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z))
        {
            return 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            return -1;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z))
        {
            Input.GetKey(KeyCode.S);
            return 0;
        }

        return 0;
    }

    private int horizontalaxis()
    {
        if (Input.GetKey(KeyCode.D))
        {
            return 1;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
        {
            return -1;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
        {
            Input.GetKey(KeyCode.D);
            return 0;
        }

        return 0;
    }

    private float Clampangle(float angle, float min, float max)
    {
        if (angle < 90f || angle > -90f)
        {
            if (angle > 180f)
            {
                angle -= 360f;
            }

            if (max > 180f)
            {
                max -= 360f;
            }

            if (min > 180f)
            {
                min -= 360f;
            }
        }

        angle = Mathf.Clamp(angle, min, max);
        if (angle < 0f)
        {
            angle += 360f;
        }

        return angle;
    }

    public void kickstate(bool state, bool vault)
    {
        if (vault)
        {
            vaultkick.SetActive(state);
            Kick.SetActive(false);
        }
        else
        {
            Kick.SetActive(state);
            vaultkick.SetActive(false);
        }

        if (state)
        {
            shootend();
            enddodge();
            gunanim.SetTrigger("kick");
            gunanim.SetBool("kicking", true);
            return;
        }

        gunanim.SetBool("kicking", false);
    }

    public void kick()
    {
        dodging = false;
        viewcam.GetComponent<Camerascript>().camershake(65.56f);
        kickdamage();
    }

    public void shootstart()
    {
        viewcam.GetComponent<Camerascript>().camershake(60.34f);
        shooting = true;
    }

    public void shootend()
    {
        shooting = false;
    }

    public void dodge()
    {
        rb.useGravity = false;
        transform.localScale -= new Vector3(0f, 0.1f, 0f);
        dashscreen.SetActive(true);
        viewcam.fieldOfView = 60f;
        gunanim.SetBool("kicking", false);
        gunanim.SetBool("moving", false);
        dodging = true;
        shooting = false;
        if (inslowmo)
        {
            var vector = (movehorizontal + moveVertical) * (dodgespeed + 100f);
            rb.velocity = new Vector3(vector.x, 0f, vector.z);
            return;
        }

        var vector2 = (movehorizontal + moveVertical) * dodgespeed;
        rb.velocity = new Vector3(vector2.x, 0f, vector2.z);
    }

    public void enddodge()
    {
        rb.useGravity = true;
        transform.localScale = origscale;
        dashscreen.SetActive(false);
        viewcam.fieldOfView = 60f;
        dodging = false;
    }

    public void Shootarrow()
    {
        Instantiate(arrow, arrowspawnpos.position, Quaternion.identity).transform.forward = viewcam.transform.forward;
    }

    public void Shootgrapplearrow()
    {
        soundmanagerscript.playsound("arrowshoot");
        Instantiate(grapplearrow, arrowspawnpos.position, Quaternion.identity).transform.forward =
            viewcam.transform.forward;
    }

    public void kickdamage()
    {
        var array = Physics.OverlapSphere(arrowspawnpos.position, kickrange, whatcanbekicked);
        if (array.Length != 0)
        {
            kickscript.kick(array, viewcam.transform.forward, transform.forward);
        }
    }

    public void takendamage()
    {
        health--;
        damagedpanel.SetActive(true);
        Invoke("resetpanel", 0.089f);
    }

    private void resetpanel()
    {
        damagedpanel.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (arrowspawnpos == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(arrowspawnpos.position, kickrange);
    }

    private void doslowmotion()
    {
        enddodge();
        shootend();
        kickstate(false, false);
        gunanim.SetFloat("Setspeedofshooting", 8.2f);
        kickanim.SetFloat("kickspeed", 8.2f);
        Time.timeScale = slowdownfactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    private void resettimescale()
    {
        gunanim.SetFloat("Setspeedofshooting", 1f);
        kickanim.SetFloat("kickspeed", 1f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    private void OnCollisionStay(Collision col)
    {
        grounded = true;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "shuriken")
        {
            takendamage();
            soundmanagerscript.playsound("playerhurt");
        }

        if (col.gameObject.tag == "Floating enemy" && !col.gameObject.GetComponent<Floatingenemyscript>().kicked &&
            !col.gameObject.GetComponent<greentargetscript>().arrowstate)
        {
            Debug.Log("Floating head did damage");
            takendamage();
            Destroy(col.gameObject, 0f);
        }

        if (col.gameObject.tag == "invisiblewall")
        {
            health = 1;
        }
    }
}