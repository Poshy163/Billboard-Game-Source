using UnityEngine;
// ReSharper disable All
public class greentargetscript : MonoBehaviour
{
    public GameObject greenarrow;

    [HideInInspector] public bool arrowstate;

    private void Start()
    {
        arrowstate = false;
    }

    private void Update()
    {
        greenarrow.SetActive(arrowstate);
    }

    public void SetArrowstate()
    {
        arrowstate = !arrowstate;
    }
}