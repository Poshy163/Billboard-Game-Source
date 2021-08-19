#region

using UnityEngine;

#endregion

public class TutorialGargole:MonoBehaviour
{
    public static bool CanDie = false;

    private void OnCollisionEnter ( Collision collision )
    {
        if(!CanDie)
        {
            return;
        }

        if(!collision.gameObject.CompareTag("arrow"))
        {
            return;
        }

        Destroy(gameObject);
    }
}