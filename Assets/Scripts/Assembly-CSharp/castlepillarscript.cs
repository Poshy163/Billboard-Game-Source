using System.Collections.Generic;
using UnityEngine;
// ReSharper disable All
public class castlepillarscript : MonoBehaviour
{
    public int numberofcastles;

    private int maxnumberofcastles;

    public List<GameObject> castles;

    private void Start()
    {
        maxnumberofcastles = 4;
        for (var i = 0; i < numberofcastles; i++)
        {
            castles[i].SetActive(true);
        }

        for (var j = numberofcastles; j < maxnumberofcastles; j++)
        {
            castles[j].SetActive(false);
        }
    }

    private void Update()
    { }
}