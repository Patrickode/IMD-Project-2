using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToShoot : MonoBehaviour
{
    public ShipControl ship;
    public float alignSpeed;
    public float triggerScale = 1.5f;

    private bool rockInRange;
    private Vector3 originalPos = new Vector3(0, -5, 0);

    private void Start()
    {
        transform.localScale = new Vector3(triggerScale, transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        if (!rockInRange)
        {
            ship.transform.position = Vector3.MoveTowards
                (
                    ship.transform.position,
                    originalPos,
                    alignSpeed * Time.deltaTime
                );
        }

        rockInRange = false;
    }

    private void OnTriggerStay(Collider rock)
    {
        if (rock.CompareTag("Rock"))
        {
            rockInRange = true;

            ship.transform.position = Vector3.MoveTowards
                (
                    ship.transform.position,
                    new Vector3(rock.gameObject.transform.position.x, originalPos.y, originalPos.z),
                    alignSpeed * Time.deltaTime
                );
        }
    }
}
