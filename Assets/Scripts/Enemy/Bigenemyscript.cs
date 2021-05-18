using UnityEngine;
// ReSharper disable All
public class Bigenemyscript : MonoBehaviour
{
    private GameObject player;

    private Color matcolor;

    private Color damagedcolor;

    private Color almostdeadcolor;

    private Animator anim;

    private Rigidbody rb;

    private bool lookatplayer;

    private float enemyspeed;

    public GameObject enemysprite;

    public float speed;

    private void Start()
    {
        anim = enemysprite.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        lookatplayer = true;
        matcolor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        damagedcolor = new Color(Color.red.r, Color.red.g, Color.red.b, 1f);
        almostdeadcolor = new Color(1f, 0f, 0f, 1f);
    }

    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        var worldPosition =
            new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        if (lookatplayer)
        {
            transform.LookAt(worldPosition);
        }

        var num = Vector3.Distance(transform.position, player.transform.position);
        if (num <= 1.1f)
        {
            anim.SetBool("moving", false);
            return;
        }

        if (num > 1.1f && num < 5.6f)
        {
            var maxDistanceDelta = Time.deltaTime * speed;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, maxDistanceDelta);
            anim.SetBool("moving", true);
            return;
        }

        if (num >= 5.6f)
        {
            anim.SetBool("moving", false);
        }
    }
}