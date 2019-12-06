using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//This gets rid of the ambiguity "Random" has when both System.Collections and UnityEngine are imported
using Random = UnityEngine.Random;

public class Vehicle : MonoBehaviour
{
    // Vectors for the physics
    private Vector3 position;
    private Vector3 direction;
    private Vector3 velocity;
    private Vector3 acceleration;

    public AgentManager manager;
    
    // The mass of the object. Note that this can't be zero
    public float mass = 1;

    public float maxSpeed = 4;
    
    public float maxForceScale = 1f;

    private const float MIN_SPEED = 0.1f;

    public GameObject target;

    private int moveMode = 0;
    private float neutralModeTimer;
    private Vector3 neutralDirection;

    /// <summary>
    /// 0 = Seek. 1 = Flee. 2 = Neutral.
    /// </summary>
    public int MoveMode
    {
        get { return moveMode; }
        set
        {
            if (value >= 0 && value <= 2)
            {
                moveMode = value;
            }
            else
            {
                //This is obviously overkill but I felt like making this extra secure for no particular reason
                Debug.LogWarning("Move mode must be between 0 and 2. Defaulted to 0.");
                moveMode = 0;
            }
        }
    }

    private void Start()
    {
        // Initialize all the vectors
        position = transform.position;
        direction = Vector3.up;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;

        neutralModeTimer = Random.Range(1f, 4f);
        neutralDirection = Vector3.zero;
    }

    private void Update()
    {
        if (moveMode == 0)
        {
            ApplyForce(Seek(target));
        }
        else if (moveMode == 1)
        {
            ApplyForce(Flee(target));
        }
        else
        {
            ApplyForce(Neutral());
        }
        
        // Then, calculate the physics
        UpdatePhysics();

        // Finally, update the position
        UpdatePosition();
    }

    /// <summary>
    /// Updates the physics properties of the vehicle
    /// </summary>
    private void UpdatePhysics()
    {
        // Add acceleration to velocity, and have that be scaled with time
        velocity += acceleration * Time.deltaTime;
        
        // Change the position based on velocity over time
        position += velocity * Time.deltaTime;
        
        // Calculate the direction vector
        direction = velocity.normalized;
        
        // Reset the acceleration for the next frame
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// Update the vehicle's position
    /// </summary>
    private void UpdatePosition()
    {
        //// Atan2 determines angle of velocity against the right vector
        //float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.up = velocity.normalized;

        // Update position
        gameObject.transform.position = position;
    }

    /// <summary>
    /// Applies friction to the vehicle
    /// </summary>
    /// <param name="coeff">The coefficient of friction</param>
    private void ApplyFriction(float coeff)
    {
        // If the velocity is below a minimum value, just stop the vehicle
        if (velocity.magnitude < MIN_SPEED)
        {
            velocity = Vector3.zero;
            return;
        }
        
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeff;
        acceleration += friction;
    }
    
    /// <summary>
    /// Applies a force to the vehicle
    /// </summary>
    /// <param name="force">The force to be applied</param>
    public void ApplyForce(Vector3 force)
    {
        // Make sure the mass isn't zero, otherwise we'll have a divide by zero error
        if (mass == 0)
        {
            Debug.LogError("Mass cannot be zero!");
            return;
        }
        
        // Add force to the acceleration for this frame
        acceleration += force / mass;
    }

    /// <summary>
    /// Returns a steering force that makes this object seek a certain position.
    /// </summary>
    /// <param name="targetPosition">The position to seek.</param>
    /// <returns>The steering force necessary to follow the target.</returns>
    private Vector3 Seek(Vector3 targetPosition)
    {
        return getSteeringForce(targetPosition - position);
    }

    private Vector3 Seek(GameObject targetObj)
    {
        return Seek(targetObj.transform.position);
    }

    /// <summary>
    /// Returns a steering force that makes this object flee from a certain position.
    /// </summary>
    /// <param name="targetPosition">The position to flee from.</param>
    /// <returns>The steering force necessary to get away from the target.</returns>
    private Vector3 Flee(Vector3 targetPosition)
    {
        return getSteeringForce(position - targetPosition);
    }

    private Vector3 Flee(GameObject targetObject)
    {
        return Flee(targetObject.transform.position);
    }

    private Vector3 Neutral()
    {
        //decrement timer
        neutralModeTimer -= Time.deltaTime;

        //when timer is up, reset it and set a new random movement vector
        if (neutralModeTimer <= 0)
        {
            neutralModeTimer = Random.Range(1f, 4f);
            neutralDirection = new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), Random.Range(-1, 2));
        }

        //Since neutralDirection is declared outside update, if it's not time yet, this uses the direction of last frame
        return getSteeringForce(neutralDirection);
    }

    private Vector3 getSteeringForce(Vector3 desiredVelocity)
    {
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;

        //Vector3 steeringForce = desiredVelocity - velocity;

        //60 is divided by the scale, so 2 will double the max force and 0.5 will half it 
        Vector3 steeringForce = (1 / (60 / maxForceScale)) * mass * (desiredVelocity - velocity) / Time.deltaTime;

        Debug.DrawLine(position, position + steeringForce);

        return steeringForce;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Make sure that mass isn't set to 0
        mass = Mathf.Max(mass, 0.0001f);
    }
    #endif 
}
