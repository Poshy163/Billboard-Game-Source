#region

using System.Collections.Generic;
using Enemy;
using UnityEngine;

#endregion

// ReSharper disable All
#pragma warning disable 414
namespace Spawners
{
    public class castlescript : MonoBehaviour
    {
        public GameObject gargoyle;

        public GameObject nextcastle;

        public Transform gargoylepos1;

        public Transform gargoylepos2;

        public bool spawninbeginning;

        [HideInInspector] public List<GameObject> spawnedgargoyle;

        public bool tospawn;

        public Material wallmaterial;

        public Material corruptedwallmaterial;

        private bool nextspawned;

        private void Start()
        {
            spawnedgargoyle = new List<GameObject>();
            nextspawned = false;
            if (tospawn)
            {
                Invoke("Spawngargoyles", 0.003f);
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

        public void Spawngargoyles()
        {
            if (gameObject.activeSelf && spawnedgargoyle.Count < 1)
            {
                spawn();
            }
        }

        public void spawn()
        {
            GameObject gameObject = Instantiate(gargoyle, gargoylepos1.position, Quaternion.identity);
            gameObject.GetComponent<gargoylescript>().postorotatearound = this.gameObject;
            spawnedgargoyle.Add(gameObject);
            GameObject gameObject2 = Instantiate(gargoyle, gargoylepos2.position, Quaternion.identity);
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
}