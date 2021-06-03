using UnityEngine;

//Formatted
// ReSharper disable All
public class looktowardsplayer:MonoBehaviour
{
    private GameObject player;

    public void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
    }

    public void Update ()
    {
        Vector3 worldPosition = new Vector3(player.transform.position.x,player.transform.position.y,player.transform.position.z);
        transform.LookAt(worldPosition);
    }
}