using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainedVibration : MonoBehaviour
{
    private Vector3 originalPos;

    public float jitterAmount;

    void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        Vector3 jitterDirection = Random.insideUnitSphere * jitterAmount;
        transform.position = (originalPos + jitterDirection);

        transform.position = new Vector3
            (
                transform.position.x,
                Mathf.Clamp(transform.position.y, 0.7f, Mathf.Infinity),
                transform.position.z
            );
    }
}
