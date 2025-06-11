using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public float rotateSpeed = 10.0f, speed = 10.0f, zoomSpeed = 10.0f;

    private float _mult = 1f;
    private bool cameraLocked = false;

    private void Update()
    {
        if (cameraLocked)
            return;

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        float rotate = 0f;
        if (Input.GetKey(KeyCode.Q))
            rotate = -1;
        else if (Input.GetKey(KeyCode.E))
            rotate = 1;

        _mult = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;

        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime * rotate * _mult, Space.World);
        transform.Translate(new Vector3(hor, 0, ver) * speed * Time.deltaTime * _mult, Space.Self);

        transform.position += transform.up * zoomSpeed * Input.GetAxis("Mouse ScrollWheel") * _mult;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -20f, 30.0f), transform.position.z);
    }

    public void SetCameraLock(bool locked)
    {
        cameraLocked = locked;
    }
}
