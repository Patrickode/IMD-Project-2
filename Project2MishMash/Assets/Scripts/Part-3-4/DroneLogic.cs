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

    private void Start()
    {
        drones = new List<GameObject>();
        drones.Add(maxNorth);
        drones.Add(minSouth);
        drones.Add(rightEast);
        drones.Add(leftWest);
    }

    void Update()
    {
        //When space is pressed, toggle logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Make sure the drones are reset during every toggle
            ZeroOutDrones();
            isPart3 = !isPart3;
        }

        SetHeightsToTerrain();

        if (isPart3)
        {
            Part3Logic();
        }
        else
        {
            Part4Logic();
        }
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

    private void ZeroOutDrones()
    {
        foreach (GameObject drone in drones)
        {
            drone.transform.position = Vector3.zero;
        }
    }
}
