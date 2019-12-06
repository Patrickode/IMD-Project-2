using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrainHeight : MonoBehaviour
{
    public float maxTerrainHeight = 25;
    public int heightResolution = 129;
    public const float initialOffset = 500;
    public float perlinCoordSpacing = 0.01f;
    public float waveSpeed = 0.01f;

    private float baseOffset;
    public List<Terrain> terrainList;
    private Terrain[,] terrains;

    void Start()
    {
        baseOffset = initialOffset;

        //initialize the terrain array, and fill it with the contents of the public list
        //This is to circumvent 2D arrays being non serializable / assignable in the editor
        terrains = new Terrain[3, 3];
        terrains[0, 0] = terrainList[0];
        terrains[1, 0] = terrainList[1];
        terrains[2, 0] = terrainList[2];
        terrains[0, 1] = terrainList[3];
        terrains[1, 1] = terrainList[4];
        terrains[2, 1] = terrainList[5];
        terrains[0, 2] = terrainList[6];
        terrains[1, 2] = terrainList[7];
        terrains[2, 2] = terrainList[8];

        for (int x = 0; x < terrains.GetLength(0); x++)
        {
            for (int y = 0; y < terrains.GetLength(1); y++)
            {
                terrains[x, y].terrainData.size = new Vector3(terrains[x, y].terrainData.size.x, maxTerrainHeight, terrains[x, y].terrainData.size.z);
                terrains[x, y].terrainData.heightmapResolution = heightResolution;
            }
        }

        //terrainData = gameObject.GetComponent<TerrainCollider>().terrainData;

        //Vector3 terrainSize = new Vector3(200, maxTerrainHeight, 200);
        //terrainData.size = terrainSize;
        //terrainData.heightmapResolution = heightResolution;

        //PerlinizeTerrain();
    }

    private void Update()
    {
        baseOffset += waveSpeed;
        TerrainFlow();
    }

    private void TerrainFlow()
    {
        for (int x = 0; x < terrains.GetLength(0); x++)
        {
            for (int y = 0; y < terrains.GetLength(1); y++)
            {
                PerlinizeTerrain
                (
                    terrains[x, y],

                    //pCS * hR = one terrain's worth of increments.
                    //Thus, multiplying by x or y adds x or y terrains of offset to the base offset
                    baseOffset + perlinCoordSpacing * heightResolution * x,
                    baseOffset + perlinCoordSpacing * heightResolution * y,

                    perlinCoordSpacing
                );
            }
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
}
