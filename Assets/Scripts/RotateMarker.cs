using UnityEngine;

public class RotateMarker : MonoBehaviour
{
    public float rotationSpeed = 600f; 

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
