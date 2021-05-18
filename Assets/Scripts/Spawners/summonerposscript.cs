using UnityEngine;
// ReSharper disable All
public class summonerposscript : MonoBehaviour
{
    public GameObject castlepiller;

    private void Start()
    { }

    private void Update()
    { }

    public void corruptpillar()
    {
        foreach (Transform transform in castlepiller.transform)
        {
            if (!(transform.name == "castle base") && transform.gameObject.activeSelf)
            {
                transform.GetComponent<castlescript>().Spawngargoyles();
            }
        }
    }

    public bool checkpillarstate()
    {
        foreach (Transform transform in castlepiller.transform)
        {
            if (!(transform.name == "castle base") && transform.gameObject.activeSelf &&
                transform.GetComponent<castlescript>().spawnedgargoyle.Count < 1)
            {
                return true;
            }
        }

        return false;
    }
}