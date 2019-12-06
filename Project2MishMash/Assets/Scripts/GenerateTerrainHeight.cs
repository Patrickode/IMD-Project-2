using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrainHeight : MonoBehaviour
{
    private TerrainData terrainData;
    private int resolution = 129;
    public float maxTerrainHeight = 50;

    // Start is called before the first frame update
    void Start()
    {
        terrainData = gameObject.GetComponent<TerrainCollider>().terrainData;

        Vector3 terrainSize = new Vector3(200, maxTerrainHeight, 200);
        terrainData.size = terrainSize;
        terrainData.heightmapResolution = resolution;

        PerlinizeTerrain();
    }

    private void PerlinizeTerrain()
    {
        //Make a height array
        float[,] heightArray = new float[resolution, resolution];

        //Fill the height array
        float xVal = 0;
        float zVal = 0;

        //For each row,
        for (int i = 0; i < heightArray.GetLength(0); i++)
        {
            //reset x val
            xVal = 0;

            //for each column,
            for (int j = 0; j < heightArray.GetLength(0); j++)
            {
                //set the height array at our current coordinate to a perlin noise value dictated by x and z vals
                heightArray[i, j] = Mathf.PerlinNoise(xVal, zVal);

                //increment x val so the perlin noise values vary like they're supposed to
                xVal += 0.05f;
            }

            //increment z val so the perlin noise values vary like they're supposed to
            zVal += 0.05f;
        }

        //finally, the terrain heightmap is done; apply it
        terrainData.SetHeights(0, 0, heightArray);
    }    
}
