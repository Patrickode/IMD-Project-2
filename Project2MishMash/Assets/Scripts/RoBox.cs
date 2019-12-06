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
    public float aDRotationAngle;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        gravity = new Vector3(0, -gravFloat, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            moveOnTrack = !moveOnTrack;

            pos = new Vector3(0f, Terrain.activeTerrain.SampleHeight(new Vector3(0f, 0f, 0f)), 0f);
            vel = new Vector3(0f, 0f, 0f);
            acc = new Vector3(0f, 0f, 0f);

            transform.position = pos;
        }

        if (moveOnTrack)
        {
            tTime +=  Time.deltaTime;
            xFloat = 50.0f * Mathf.Cos(tTime);
            zFloat = 50.0f * Mathf.Sin(tTime);
            yFloat = (1f/100f) * xFloat * zFloat;

            //this is another way to get the y value that will work for any heightmap-specified surface of the form y = f(x,z) 
            //y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0f, z)) - 25f; //Note that -25f adjusts for the translation of the Terrain GameObject 

            parametric = new Vector3(xFloat, yFloat, zFloat);
            transform.position = parametric;

            //the gradient to the surface F(x,y,z) = 0 is the normal vector of the local tangent plane to the surface, which here is F(x,y,z) = y - x z
            gradient = new Vector3(-zFloat/100f, 1, -xFloat/100f);  //the gradient is just the vector of the derivatives < dF/dx, dF/dy, dF/dz >
          
            vel = new Vector3(-50.0f * Mathf.Sin(tTime), 25.0f * Mathf.Cos(2.0f * tTime), 50.0f * Mathf.Cos(tTime));

            //this is another way to get the normal vector that will work for any heightmap-specified surface of the form y = f(x,z)
            //transform.up = Terrain.activeTerrain.GetComponent<TerrainCollider>().terrainData.GetInterpolatedNormal((x+50f)/100f, (z+50f)/100f);

            transform.up = gradient.normalized;
            //transform.right = Vector3.Cross(gradient.normalized, vel.normalized); //this isn't necessary since Unity computes the 3rd vector given the other 2
            transform.forward = vel.normalized;
        }

        //when not moving on track, move with wasd
        else
        {
            transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) - 25, transform.position.z);

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

            if (vel.magnitude < 0.02f)
            {
                vel = Vector3.zero;
            }

            transform.position += vel * Time.deltaTime;

            //Get the normal of the terrain so we can set the rotation of the object with it.
            Vector3 terrainNormal = GetTerrainNormal(Terrain.activeTerrain, transform.position);

            //LookRotation prioritizes the forward vector, so we have to make our forward vector perpendicular
            //to the normal, thus properly aligning the object with the normal
            vel = vel - terrainNormal * Vector3.Dot(vel, terrainNormal);

            if (vel.magnitude >= 0.02f)
            {
                transform.rotation = Quaternion.LookRotation(vel.normalized, terrainNormal);
            }
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
    public Vector3 GetTerrainNormal(Terrain terrain, Vector3 position)
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
