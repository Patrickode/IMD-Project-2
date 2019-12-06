using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoBox : MonoBehaviour
{
    private Vector3 pos;
    private Vector3 vel;
    private Vector3 acc;
    private float gravFloat = 32f;
    private Vector3 gravity;
    private bool moveOnTrack = false;
    private float xFloat;
    private float yFloat;
    private float zFloat;

    private Vector3 zero = new Vector3(0f, 0f, 0f);
    private Vector3 origin = new Vector3(0f, 0f, 0f);
    private float tTime = 0f;
    private Vector3 gradient, parametric;

    public float accelScale;
    public float decelScale;
    public float aDRotationAngle;
    private float speed;

    public Terrain centralTerrain;

    // Start is called before the first frame update
    void Start()
    {
        gravity = new Vector3(0, -gravFloat, 0);
    }

    // Update is called once per frame
    void Update()
    {
        MoveLogic();

        //after move logic is executed, set position to be within the range of the center terrain
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 1, 99), transform.position.y, Mathf.Clamp(transform.position.z, 1, 99));
    }

    private void MoveLogic()
    {
        //Set the height to be the height of the terrain (I found the long decimal myself to make the bottom mostly align)
        transform.position = new Vector3(transform.position.x, centralTerrain.SampleHeight(transform.position) + 0.82048f, transform.position.z);

        //forward and back
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            acc = transform.forward * accelScale;

            vel += acc * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            if (vel.magnitude >= 0.15f)
            {
                acc = -transform.forward * accelScale;

                vel += acc * Time.deltaTime;
            }
            else
            {
                acc = Vector3.zero;
            }
        }

        //right and left
        else if (Input.GetKey(KeyCode.RightArrow) && vel.magnitude >= 0.02f)
        {
            acc = transform.right * accelScale;

            speed = vel.magnitude;
            vel += acc * Time.deltaTime;
            vel = speed * vel.normalized;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && vel.magnitude >= 0.02f)
        {
            acc = -transform.right * accelScale;

            speed = vel.magnitude;
            vel += acc * Time.deltaTime;
            vel = speed * vel.normalized;
        }

        //alt right and left
        else if (Input.GetKey(KeyCode.D) && vel.magnitude >= 0.02f)
        {
            vel = Quaternion.Euler(0, aDRotationAngle, 0) * vel;
        }
        else if (Input.GetKey(KeyCode.A) && vel.magnitude >= 0.02f)
        {
            vel = Quaternion.Euler(0, -aDRotationAngle, 0) * vel;
        }

        else
        {
            acc = Vector3.zero;
        }
        
        //while acceleration is zero, start slowing down.
        if (acc == Vector3.zero)
        {
            //velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * decelScale);

            //Constantly subtract from velocity until it reaches a certain threshold. At said threshold, stop. (i.e., stop when "close enough" to zero)
            if (Vector3.Distance(vel - vel * decelScale, Vector3.zero) >= 0.15)
            {
                vel -= vel * decelScale;
            }
            else
            {
                vel = Vector3.zero;
            }
        }

        transform.position += vel * Time.deltaTime;

        //Get the normal of the terrain so we can set the rotation of the object with it.
        Vector3 terrainNormal = GetTerrainNormal(centralTerrain, transform.position);

        //LookRotation prioritizes the forward vector, so we have to make our forward vector perpendicular
        //to the normal, thus properly aligning the object with the normal
        vel = vel - terrainNormal * Vector3.Dot(vel, terrainNormal);

        Debug.DrawRay(transform.position, terrainNormal, Color.magenta);
        Debug.DrawRay(transform.position, vel, Color.yellow);

        if (vel.magnitude >= 0.02f)
        {
            transform.rotation = Quaternion.LookRotation(vel.normalized, terrainNormal);
        }
        else
        {
            transform.forward = transform.forward - terrainNormal * Vector3.Dot(transform.forward, terrainNormal);
            transform.rotation = Quaternion.LookRotation(transform.forward, terrainNormal);
        }
    }

    /// <summary>
    /// Takes a terrain and a position, and gets the normal of the terrain at that position.
    /// Huge thanks to http://answers.unity.com/answers/1295326/view.html for this method; it's been
    /// slightly edited to fit this script, but the idea of using InverseLerp and whatnot came from there.
    /// </summary>
    /// <param name="terrain">The terrain to get the normal from.</param>
    /// <param name="position">The position on the terrain to get the normal from.</param>
    /// <returns>The normal at the given position on the given terrain.</returns>
    private Vector3 GetTerrainNormal(Terrain terrain, Vector3 position)
    {
        //Subtracts the terrain's position from the given position, to get the position relative to the terrain
        Vector3 terrainLocalPos = position - terrain.transform.position;

        //with that relative position, use InverseLerp (get percentage from value) to get how far along the terrain
        //the relative position is, from 0 to 1
        Vector2 normalizedPos = new Vector2(
            Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
            Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z)
        );

        //Finally, now that we have the normalized, local position, we can get the interpolated normal at that position
        var terrainNormal = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
        return terrainNormal;
    }
}
