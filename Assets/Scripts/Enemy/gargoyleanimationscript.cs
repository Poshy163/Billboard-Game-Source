using UnityEngine;
// ReSharper disable All
public class gargoyleanimationscript : MonoBehaviour
{
    private gargoylescript gargoyle;

    private void Start ()
    {
        gargoyle = transform.parent.gameObject.GetComponent<gargoylescript>();
    }
    private void shoot ()
    {
        gargoyle.shoot();
    }
}