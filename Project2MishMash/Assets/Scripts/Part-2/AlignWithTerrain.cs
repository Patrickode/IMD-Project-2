using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithTerrain : MonoBehaviour
{
    Vector3 terrainNormal;

    void Start()
    {
        //When this object is spawned, make sure it's level with the terrain.
        transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + 0.5f, transform.position.z);

        //Once level, rotate to be aligned with the terrain.
        terrainNormal = GetTerrainNormal(Terrain.activeTerrain, transform.position);
        transform.forward = transform.forward - terrainNormal * Vector3.Dot(transform.forward, terrainNormal);
        transform.rotation = Quaternion.LookRotation(transform.forward, terrainNormal);
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
