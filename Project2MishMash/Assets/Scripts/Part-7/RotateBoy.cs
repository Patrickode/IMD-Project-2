using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBoy : MonoBehaviour
{
    public float rotateSpeed;
    public GameObject cam;
    public GameObject victim;

    void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));

        cam.transform.LookAt(victim.transform);
    }
}
