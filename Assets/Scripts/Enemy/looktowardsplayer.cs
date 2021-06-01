using UnityEngine;

//Formatted
// ReSharper disable All
public class looktowardsplayer : MonoBehaviour
{
    private GameObject player;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        var worldPosition = new Vector3(player.transform.position.x, player.transform.position.y,
            player.transform.position.z);
        transform.LookAt(worldPosition);
    }
}