using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTerrain : MonoBehaviour
{
    public float maxTerrainHeight = 25;
    public const float initialOffset = 500;
    public float perlinCoordSpacing = 0.01f;
    public Terrain terrain;

    void Start()
    {
        terrain.terrainData.size = new Vector3(terrain.terrainData.size.x, maxTerrainHeight, terrain.terrainData.size.z);

        PerlinizeTerrain(terrain, initialOffset, initialOffset, perlinCoordSpacing);
    }

    private void PerlinizeTerrain(Terrain terrainToPerlinize, float xOffset, float yOffset, float spacing)
    {
        //Make a height array
        float[,] heightArray = new float[terrainToPerlinize.terrainData.heightmapResolution, terrainToPerlinize.terrainData.heightmapResolution];

        //Fill the height array
        float xVal = xOffset;
        float zVal = yOffset;

        //For each row,
        for (int i = 0; i < heightArray.GetLength(0); i++)
        {
            //reset x val, because we're at a new row
            xVal = xOffset;

            //for each column,
            for (int j = 0; j < heightArray.GetLength(0); j++)
            {
                //set the height array at our current coordinate to a perlin noise value dictated by x and z vals
                heightArray[i, j] = Mathf.PerlinNoise(xVal, zVal);

                //move over to the next x coordinate
                xVal += spacing;
            }

            //move onto the next z coordinate
            zVal += spacing;
        }

        //finally, the terrain heightmap is done; apply it
        terrainToPerlinize.terrainData.SetHeights(0, 0, heightArray);
    }
}
