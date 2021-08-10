﻿using UnityEngine;

public class TutorialGargole:MonoBehaviour
{
    private void OnCollisionEnter ( Collision collision )
    {
        if(!collision.gameObject.CompareTag("arrow"))
        {
            return;
        }

        Destroy(gameObject);
    }
}
