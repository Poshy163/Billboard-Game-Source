using Spawners;
using UnityEngine;

// ReSharper disable All
namespace Enemy
{
    public class summonerposscript:MonoBehaviour
    {
        public GameObject castlepiller;

        public void corruptpillar ()
        {
            foreach(Transform transform in castlepiller.transform)
            {
                if(!(transform.name == "castle base") && transform.gameObject.activeSelf)
                {
                    transform.GetComponent<castlescript>().Spawngargoyles();
                }
            }
        }

        public bool checkpillarstate ()
        {
            foreach(Transform transform in castlepiller.transform)
            {
                if(!(transform.name == "castle base") && transform.gameObject.activeSelf &&
                    transform.GetComponent<castlescript>().spawnedgargoyle.Count < 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}