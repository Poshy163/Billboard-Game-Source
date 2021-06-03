using UnityEngine;

//ReSharper disable All
//Formatted
public class Bigenemyscript:MonoBehaviour
{
    public GameObject enemysprite;

    public float speed;

    private Color almostdeadcolor = new Color(1f,0f,0f,1f);

    private Animator anim;

    private Color damagedcolor = new Color(Color.red.r,Color.red.g,Color.red.b,1f);

    private readonly bool lookatplayer = true;

    private Color matcolor;
    private GameObject player;

    private Rigidbody rb;

    public void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = enemysprite.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        matcolor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
    }

    public void Update ()
    {
        Vector3 worldPosition = new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z);
        if(lookatplayer)
        {
            transform.LookAt(worldPosition);
        }

        float num = Vector3.Distance(transform.position,player.transform.position);
        if(num <= 1.1f)
        {
            anim.SetBool("moving",false);
            return;
        }

        if(num > 1.1f && num < 5.6f)
        {
            float maxDistanceDelta = Time.deltaTime * speed;
            transform.position = Vector3.MoveTowards(transform.position,player.transform.position,maxDistanceDelta);
            anim.SetBool("moving",true);
            return;
        }

        if(num >= 5.6f)
        {
            anim.SetBool("moving",false);
        }
    }
}