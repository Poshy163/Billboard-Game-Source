using UnityEngine;

// ReSharper disable All
public class playeranimationfunctions:MonoBehaviour
{
    private PlayerController controller;

    private void Start ()
    {
        controller = transform.parent.transform.parent.GetComponent<PlayerController>();
    }

    private void Update ()
    { }

    private void startdodge ()
    {
        controller.dodge();
    }

    private void enddodge ()
    {
        controller.enddodge();
    }

    private void kick ()
    {
        controller.kick();
    }

    private void endkick ()
    {
        controller.kickstate(false,false);
    }

    private void shootstart ()
    {
        controller.shootstart();
    }

    private void shoot ()
    {
        controller.Shootarrow();
    }

    private void shootgrapplearrow ()
    {
        controller.Shootgrapplearrow();
    }

    private void shootend ()
    {
        controller.shootend();
    }
}