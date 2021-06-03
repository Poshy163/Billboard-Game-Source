using UnityEngine;

// ReSharper disable All
public class kickscript:MonoBehaviour
{
    public static void kick ( Collider[] objectshit,Vector3 cameradir,Vector3 playerdir )
    {
        for(int i = 0;i < objectshit.Length;i++)
        {
            Collider collider = objectshit[i];
            string tag = collider.gameObject.tag;
            if(!(tag == "Fodder"))
            {
                if(!(tag == "Gargoyle"))
                {
                    if(tag == "Floating enemy")
                    {
                        if(!collider.gameObject.GetComponent<Floatingenemyscript>().kicked)
                        {
                            collider.gameObject.GetComponent<Floatingenemyscript>().enemykickedback(cameradir);
                        }
                    }
                }
                else if(!collider.transform.parent.GetComponent<gargoylescript>().kicked)
                {
                    collider.transform.parent.GetComponent<gargoylescript>().enemykickedback(20000f);
                }
            }
        }
    }

    public static bool enemyhitbykickedenemy ( GameObject kickedenemy )
    {
        bool result = false;
        string tag = kickedenemy.tag;
        if(!(tag == "Fodder"))
        {
            if(tag == "Gargoyle")
            {
                if(kickedenemy.GetComponent<gargoylescript>().kicked)
                {
                    result = true;
                }
            }
        }

        return result;
    }
}