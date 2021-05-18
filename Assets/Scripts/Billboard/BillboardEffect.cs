using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardEffect : MonoBehaviour
{

    private new Camera camera;
   
   void Start()
   {
        camera = Camera.main;
   }

    private void LateUpdate ()
    {
        transform.LookAt(camera.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

}
