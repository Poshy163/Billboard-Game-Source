using UnityEngine;

// ReSharper disable All
public class Summoneranimationscript : MonoBehaviour
{
    private Summonerscript summoner;

    private void Start()
    {
        summoner = transform.parent.gameObject.GetComponent<Summonerscript>();
    }

    private void shootstart()
    {
        summoner.shootstart();
    }

    private void spawnhead()
    {
        summoner.spawnhead();
    }

    private void shoot()
    {
        summoner.shoot();
    }


    private void shootend()
    {
        summoner.shootend();
    }
}