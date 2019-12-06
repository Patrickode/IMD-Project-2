using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishControls : MonoBehaviour
{
    private Vector3 acceleration;
    private Vector3 velocity;
    private Vector3 position;
    private float speed;
    private bool autoPilot;

    public AgentManager manager;

    public float accelScale = 1;
    public float decelScale = 1;
    public float aDRotationAngle;

    private void OnEnable()
    {
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        position = transform.position;
    }

    void Update()
    {
        //forward and back
        if (Input.GetKey(KeyCode.W))
        {
            acceleration = transform.up * accelScale;

            velocity += acceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (velocity.magnitude >= 0.15f)
            {
                acceleration = -transform.up * accelScale;

                velocity += acceleration * Time.deltaTime;
            }
            else
            {
                acceleration = Vector3.zero;
            }
        }

        //right and left
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            //TurnVelocity(transform.right);
            speed = velocity.magnitude;
            velocity = Quaternion.Euler(0, 0, -aDRotationAngle) * velocity;
            velocity = velocity.normalized * speed;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            //TurnVelocity(-transform.right);
            speed = velocity.magnitude;
            velocity = Quaternion.Euler(0, 0, aDRotationAngle) * velocity;
            velocity = velocity.normalized * speed;
        }

        ////alt right and left
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    velocity = Quaternion.Euler(0, 0, -aDRotationAngle) * velocity;
        //}
        //else if (Input.GetKey(KeyCode.A))
        //{
        //    velocity = Quaternion.Euler(0, 0, aDRotationAngle) * velocity;
        //}

        //up and down
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            //TurnVelocity(-transform.forward);
            speed = velocity.magnitude;
            velocity = Quaternion.Euler(-aDRotationAngle, 0, 0) * velocity;
            velocity = velocity.normalized * speed;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //TurnVelocity(transform.forward);
            speed = velocity.magnitude;
            velocity = Quaternion.Euler(aDRotationAngle, 0, 0) * velocity;
            velocity = velocity.normalized * speed;
        }

        else
        {
            acceleration = Vector3.zero;
        }

        //while acceleration is zero, start slowing down.
        if (acceleration == Vector3.zero)
        {
            //velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * decelScale);

            //Constantly subtract from velocity until it reaches a certain threshold. At said threshold, stop. (i.e., stop when "close enough" to zero)
            if (Vector3.Distance(velocity - velocity * decelScale, Vector3.zero) >= 0.15)
            {
                velocity -= velocity * decelScale;
            }
            else
            {
                velocity = Vector3.zero;
            }
        }

        //actually move the object with the velocity we calculated.
        position += velocity * Time.deltaTime;
        transform.position = position;

        //rotate to the direction of velocity. Unity unleashes a torrent of console warnings if v.norm is zero
        if (velocity.normalized != Vector3.zero)
        {
            transform.up = velocity.normalized;
        }

        //Finally, make sure position is in bounds.
        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, -49, 49),
            Mathf.Clamp(transform.position.y, Terrain.activeTerrain.SampleHeight(transform.position) + 0.5f, 97),
            Mathf.Clamp(transform.position.z, -49, 49)
        );
    }

    /// <summary>
    /// Turns the object toward a given direction.
    /// </summary>
    /// <param name="refVector">The direction of acceleration.</param>
    private void TurnVelocity(Vector3 refVector)
    {
        acceleration = refVector * accelScale;

        speed = velocity.magnitude;
        velocity += acceleration * Time.deltaTime;
        velocity = speed * velocity.normalized;
    }
}
