using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPEEN : MonoBehaviour
{
    public GameManager manager;
    public float spinSpeed;

    void Update()
    {
        if (!manager.gameOverText.enabled)
        {
            transform.Rotate(0, 0, spinSpeed);
        }
    }
}