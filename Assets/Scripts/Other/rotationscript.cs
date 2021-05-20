using UnityEngine;
// ReSharper disable All
public class rotationscript : MonoBehaviour
{
    private GameObject player;

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, 0f, 18f), Space.Self);
    }
}