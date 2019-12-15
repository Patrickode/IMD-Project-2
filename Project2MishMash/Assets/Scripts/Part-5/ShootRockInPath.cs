using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRockInPath : MonoBehaviour
{
    public ShipControl ship;

    private void OnTriggerEnter(Collider rock)
    {
        if (rock.CompareTag("Rock"))
        {
            ship.Shoot();
        }
    }
}
