using UnityEngine;
// ReSharper disable All
public class looktowardsplayer : MonoBehaviour
{
    private GameObject player;

    private void Start()
    { }

    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        var worldPosition = new Vector3(player.transform.position.x, player.transform.position.y,
            player.transform.position.z);
        transform.LookAt(worldPosition);
    }
}