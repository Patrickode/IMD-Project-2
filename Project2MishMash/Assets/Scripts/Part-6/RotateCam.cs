﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCam : MonoBehaviour
{
    public float rotateSpeed;
    public GameObject camera;

    void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));

        camera.transform.LookAt(transform);
    }
}
