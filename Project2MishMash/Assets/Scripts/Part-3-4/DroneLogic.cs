using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneLogic : MonoBehaviour
{
    public float droneSpeed;

    public GameObject maxNorth;
    public GameObject minSouth;
    public GameObject rightEast;
    public GameObject leftWest;
    public Terrain terrain;

    private List<GameObject> drones;
    private bool isPart3 = true;
    private bool paused;

    private int part4Step = 1;
    private float currentAngle;
    private Vector3 localMin;
    private Vector3 localMax;
    
    /// <summary>
    /// The angle of maxNorth's spoke.
    /// </summary>
    private float maNoAngle;

    /// <summary>
    /// The angle of minSouth's spoke.
    /// </summary>
    private float miSoAngle;

    /// <summary>
    /// The angle of rightEast's spoke.
    /// </summary>
    private float riEaAngle;

    /// <summary>
    /// The angle of leftWest's spoke.
    /// </summary>
    private float leWeAngle;

    private void Start()
    {
        drones = new List<GameObject>();
        drones.Add(maxNorth);
        drones.Add(minSouth);
        drones.Add(rightEast);
        drones.Add(leftWest);

        localMin = new Vector3(0, 1000, 0);
        localMax = new Vector3(0, -1000, 0);
    }

    void Update()
    {
        //When space is pressed, toggle logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Make sure the drones are reset during every toggle
            ResetLogic();
            isPart3 = !isPart3;
        }

        //When p is pressed, pause logic entirely
        if (Input.GetKeyDown(KeyCode.P))
        {
            paused = !paused;
        }

        SetHeightsToTerrain();

        if (isPart3 && !paused)
        {
            Part3Logic();
        }
        else if (!paused)
        {
            Part4Logic();
        }

        Debug.DrawRay(localMin, Vector3.up * 100, Color.blue);
        Debug.DrawRay(localMax, Vector3.up * 100, Color.red);
    }

    private void Part3Logic()
    {
        //Max Logic
        Vector3 maxDroneNormal = GetTerrainNormal(terrain, maxNorth.transform.position);
        Vector3 ascentDir = new Vector3(-maxDroneNormal.x, 0, -maxDroneNormal.z);
        if (Vector3.Distance(ascentDir, Vector3.zero) >= 0.05f)
        {
            NormalizedTranslate(maxNorth, ascentDir, droneSpeed);
        }

        //Min Logic
        Vector3 minDroneNormal = GetTerrainNormal(terrain, minSouth.transform.position);
        Vector3 descentDir = new Vector3(minDroneNormal.x, 0, minDroneNormal.z);
        if (Vector3.Distance(descentDir, Vector3.zero) >= 0.05f)
        {
            NormalizedTranslate(minSouth, descentDir, droneSpeed);
        }

        //Right Logic
        Vector3 rightDroneNormal = GetTerrainNormal(terrain, rightEast.transform.position);
        Vector3 rightLevelDir = new Vector3(-rightDroneNormal.z, 0, rightDroneNormal.x);
        NormalizedTranslate(rightEast, rightLevelDir, droneSpeed);

        //Left Logic
        Vector3 leftDroneNormal = GetTerrainNormal(terrain, leftWest.transform.position);
        Vector3 leftLevelDir = new Vector3(leftDroneNormal.z, 0, -leftDroneNormal.x);
        NormalizedTranslate(leftWest, leftLevelDir, droneSpeed);
    }

    private void Part4Logic()
    {
        //Step 1: All the drones are sent out up to a point, because this terrain is biglarge
        if (part4Step == 1)
        {
            //We want each drone to be ~50 units away from the origin.
            bool northAtDest = maxNorth.transform.position.magnitude >= 50;
            bool southAtDest = minSouth.transform.position.magnitude >= 50;
            bool eastAtDest = rightEast.transform.position.magnitude >= 50;
            bool westAtDest = leftWest.transform.position.magnitude >= 50;

            //If all the drones are where they need to be, move on
            if (northAtDest && southAtDest && eastAtDest && westAtDest)
            {
                part4Step++;
                Debug.Log("Next Step: " + part4Step);
            }

            //Otherwise, move the drones until they are.
            else
            {
                if (!northAtDest)
                {
                    NormalizedTranslate(maxNorth, Vector3.forward, droneSpeed * 2);
                }
                if (!southAtDest)
                {
                    NormalizedTranslate(minSouth, -Vector3.forward, droneSpeed * 2);
                }
                if (!eastAtDest)
                {
                    NormalizedTranslate(rightEast, Vector3.right, droneSpeed * 2);
                }
                if (!westAtDest)
                {
                    NormalizedTranslate(leftWest, -Vector3.right, droneSpeed * 2);
                }
            }
        }

        //Step 2: Revolve in a circle and find the min and max of said circle.
        else if (part4Step == 2)
        {
            //Move each of the drones 90 degrees along the circle between their positions.
            if (currentAngle < 90)
            {
                currentAngle += droneSpeed * 5;
                transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
            }
            else
            {
                part4Step++;
                Debug.Log("Next Step: " + part4Step);
            }

            //While the drones move, constantly check to see if any of the drones have hit a new min or max.
            //min
            UpdateLocalExtreme(true, maxNorth.transform.position);
            UpdateLocalExtreme(true, minSouth.transform.position);
            UpdateLocalExtreme(true, rightEast.transform.position);
            UpdateLocalExtreme(true, leftWest.transform.position);
            //max
            UpdateLocalExtreme(false, maxNorth.transform.position);
            UpdateLocalExtreme(false, minSouth.transform.position);
            UpdateLocalExtreme(false, rightEast.transform.position);
            UpdateLocalExtreme(false, leftWest.transform.position);
        }

        //Step 3: Move each drone to either the max or min, depending.
        else if (part4Step == 3)
        {
            //We want north and east to be at the max, and south and west to be at the min.
            bool northAtDest = Vector3.Distance(maxNorth.transform.position, localMax) <= 0.5f;
            bool southAtDest = Vector3.Distance(minSouth.transform.position, localMin) <= 0.5f;
            bool eastAtDest = Vector3.Distance(rightEast.transform.position, localMax) <= 0.5f;
            bool westAtDest = Vector3.Distance(leftWest.transform.position, localMin) <= 0.5f;

            //If all the drones are where they need to be, move on
            if (northAtDest && southAtDest && eastAtDest && westAtDest)
            {
                part4Step++;
                Debug.Log("Next Step: " + part4Step);
            }

            //Otherwise, move the drones until they are.
            else
            {
                if (!northAtDest)
                {
                    maNoAngle += 0.25f;
                    maxNorth.transform.parent.rotation = Quaternion.AngleAxis(maNoAngle, Vector3.up);
                }
                if (!southAtDest)
                {
                    miSoAngle += 0.25f;
                    minSouth.transform.parent.rotation = Quaternion.AngleAxis(miSoAngle, Vector3.up);
                }
                if (!eastAtDest)
                {
                    riEaAngle += 0.25f;
                    rightEast.transform.parent.rotation = Quaternion.AngleAxis(riEaAngle, Vector3.up);
                }
                if (!westAtDest)
                {
                    leWeAngle += 0.25f;
                    leftWest.transform.parent.rotation = Quaternion.AngleAxis(leWeAngle, Vector3.up);
                }
            }
        }
    }

    /// <summary>
    /// Sets the heights of all the drones to be the height of the terrain.
    /// </summary>
    private void SetHeightsToTerrain()
    {
        foreach (GameObject drone in drones)
        {
            drone.transform.position = new Vector3
                (
                    drone.transform.position.x,
                    terrain.SampleHeight(drone.transform.position),
                    drone.transform.position.z
                );
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

    private void NormalizedTranslate(GameObject objToMove, Vector3 direction, float speedToMove)
    {
        objToMove.transform.Translate(direction.normalized * speedToMove);
    }

    private void ResetLogic()
    {
        //Reset the step Part 4 is at so it starts at the beginning.
        part4Step = 1;

        //Set local min and max to some extreme values so any point will overwrite them.
        localMin = new Vector3(0, 1000, 0);
        localMax = new Vector3(0, -1000, 0);

        //Reset the manager's rotation.
        transform.rotation = Quaternion.identity;

        //Reset all the drones' positions, and the rotation of their spokes.
        foreach (GameObject drone in drones)
        {
            drone.transform.parent.rotation = Quaternion.identity;
            drone.transform.position = Vector3.zero;
        }

        //Reset the angle counters to match the reset rotations.
        currentAngle = 0;
        maNoAngle = 0;
        miSoAngle = 0;
        riEaAngle = 0;
        leWeAngle = 0;
    }

    private void UpdateLocalExtreme(bool isMin, Vector3 posToCompare)
    {
        if (isMin)
        {
            if (posToCompare.y < localMin.y)
            {
                localMin = posToCompare;
            }
        }
        else
        {
            if (posToCompare.y > localMax.y)
            {
                localMax = posToCompare;
            }
        }
    }
}
