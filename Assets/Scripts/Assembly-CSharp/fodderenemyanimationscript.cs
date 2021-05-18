using UnityEngine;
// ReSharper disable All
public class fodderenemyanimationscript : MonoBehaviour
{
    private fodderenemyscript fodder;

    private void Start()
    {
        fodder = transform.parent.gameObject.GetComponent<fodderenemyscript>();
    }

    private void jumpstart()
    {
        fodder.jumping = true;
    }

    private void jumpsetpos()
    {
        fodder.jumpsetpos();
    }

    private void jumpend()
    {
        fodder.jumping = false;
        fodder.posset = false;
        gameObject.GetComponent<Animator>().SetBool("jumpend", false);
    }

    private void startslash()
    {
        fodder.startslash();
    }

    private void slash()
    {
        fodder.slash();
    }

    private void endslash()
    {
        fodder.endslash();
    }

    private void shurikenthrow()
    {
        fodder.shurikenthrow();
    }
}