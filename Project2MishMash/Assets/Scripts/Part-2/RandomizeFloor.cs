using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeFloor : MonoBehaviour
{
    public GameObject piecePrefab;

    void Start()
    {
        PerlinizeTerrain(Terrain.activeTerrain, 0, 0, 0.01f);

        //summon a sizeable number of chess pieces to populate this sparse ocean landscape
        int numberOfBoys = Random.Range(100, 201);
        int mean = 0;

        for (int i = 0; i < numberOfBoys; i++)
        {
            Instantiate
                (
                    piecePrefab,
                    new Vector3(Gaussian(mean, mean + 49), 0, Gaussian(mean, mean + 49)),
                    Quaternion.identity
                );
        }
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

    /// <summary>
    /// Takes a mean and a standard deviation, and returns a random value according to the bell curve defined by them.
    /// </summary>
    /// <param name="mean">The average value of the bell curve.</param>
    /// <param name="maxDifference">The maximum difference from the mean. Standard deviation is a 4th of this.</param>
    /// <returns>A random value somewhere on the bell curve; likely within 1 deviation.</returns>
    private float Gaussian(float mean, float maxDifference)
    {
        float val1 = Random.Range(0f, 1f);
        float val2 = Random.Range(0f, 1f);

        float gaussValue = Mathf.Sqrt(-2.0f * Mathf.Log(val1)) * Mathf.Sin(2.0f * Mathf.PI * val2);

        return mean + (maxDifference / 8) * gaussValue;
    }
}
