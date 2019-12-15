using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToShoot : MonoBehaviour
{
    public ShipControl ship;
    private bool rockInRange;
    private Vector3 originalPos = new Vector3(0, -5, 0);

    void Update()
    {
        if (!rockInRange && Mathf.Approximately(Vector3.Distance(originalPos, ship.gameObject.transform.position), 0))
        {
            ship.transform.position = Vector3.MoveTowards
                (
                    ship.transform.position,
                    originalPos,
                    2f * Time.deltaTime
                );

            //Vector3 desiredVector = originalPos - ship.gameObject.transform.position;
            //ship.gameObject.transform.Translate
            //    (
            //        new Vector3(desiredVector.normalized.x * ship.speed * Time.deltaTime, 0, 0)
            //    );
        }
    }

    private void OnTriggerStay(Collider rock)
    {
        if (rock.CompareTag("Rock"))
        {
            Debug.Log("Rock is in range");
            rockInRange = true;

            ship.transform.position = Vector3.MoveTowards
                (
                    ship.transform.position,
                    new Vector3(rock.gameObject.transform.position.x, originalPos.y, originalPos.z),
                    2f * Time.deltaTime
                );
        }
    }
}
