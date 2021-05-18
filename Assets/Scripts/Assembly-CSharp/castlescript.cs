using System.Collections.Generic;
using UnityEngine;
// ReSharper disable All
public class castlescript : MonoBehaviour
{
    public GameObject gargoyle;

    public GameObject nextcastle;

    public Transform gargoylepos1;

    public Transform gargoylepos2;

    public bool spawninbeginning;

    [HideInInspector] public List<GameObject> spawnedgargoyle;

    private bool nextspawned;

    public bool tospawn;

    public Material wallmaterial;

    public Material corruptedwallmaterial;

    private void Start()
    {
        spawnedgargoyle = new List<GameObject>();
        nextspawned = false;
        if (tospawn)
        {
            Invoke("Spawngargoyles", 0.003f);
        }
    }

    public void Spawngargoyles()
    {
        if (gameObject.activeSelf && spawnedgargoyle.Count < 1)
        {
            spawn();
        }
    }

    private void Update()
    {
        if (spawnedgargoyle.Count > 0)
        {
            gameObject.GetComponent<MeshRenderer>().material = corruptedwallmaterial;
            return;
        }

        gameObject.GetComponent<MeshRenderer>().material = wallmaterial;
    }

    public void spawn()
    {
        Debug.Log(Random.Range(0f, 1f));
        var gameObject = Instantiate(gargoyle, gargoylepos1.position, Quaternion.identity);
        gameObject.GetComponent<gargoylescript>().postorotatearound = this.gameObject;
        spawnedgargoyle.Add(gameObject);
        var gameObject2 = Instantiate(gargoyle, gargoylepos2.position, Quaternion.identity);
        gameObject2.GetComponent<gargoylescript>().postorotatearound = this.gameObject;
        spawnedgargoyle.Add(gameObject2);
    }

    public void gargoyledead()
    {
        if (nextcastle != null)
        {
            nextspawned = true;
            nextcastle.GetComponent<castlescript>().spawn();
            nextcastle.GetComponent<castlescript>().tospawn = true;
            nextcastle = null;
        }
    }
}