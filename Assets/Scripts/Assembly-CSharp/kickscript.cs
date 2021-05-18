using UnityEngine;
// ReSharper disable All
public class kickscript : MonoBehaviour
{
    public static void kick(Collider[] objectshit, Vector3 cameradir, Vector3 playerdir)
    {
        for (var i = 0; i < objectshit.Length; i++)
        {
            var collider = objectshit[i];
            var tag = collider.gameObject.tag;
            if (!(tag == "Fodder"))
            {
                if (!(tag == "Gargoyle"))
                {
                    if (tag == "Floating enemy")
                    {
                        if (!collider.gameObject.GetComponent<Floatingenemyscript>().kicked)
                        {
                            collider.gameObject.GetComponent<Floatingenemyscript>().enemykickedback(cameradir);
                        }
                    }
                }
                else if (!collider.transform.parent.GetComponent<gargoylescript>().kicked)
                {
                    collider.transform.parent.GetComponent<gargoylescript>().enemykickedback(20000f);
                }
            }
            else if (!collider.transform.parent.gameObject.GetComponent<fodderenemyscript>().kicked)
            {
                collider.transform.parent.gameObject.GetComponent<fodderenemyscript>().enemykickedback(29140f);
            }
        }
    }

    public static bool enemyhitbykickedenemy(GameObject kickedenemy)
    {
        var result = false;
        var tag = kickedenemy.tag;
        if (!(tag == "Fodder"))
        {
            if (tag == "Gargoyle")
            {
                if (kickedenemy.GetComponent<gargoylescript>().kicked)
                {
                    result = true;
                }
            }
        }
        else if (kickedenemy.GetComponent<fodderenemyscript>().kicked)
        {
            result = true;
        }

        return result;
    }
}