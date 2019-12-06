using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TerrainGeneration : MonoBehaviour 
{
	private TerrainData myTerrainData;
	public Vector3 worldSize;
	public int resolution = 129;			// number of vertices along X and Z axes
	float[,] heightArray;

    public GameObject mainCamera;
    public GameObject vehicle;

	void Start () 
	{
		myTerrainData = gameObject.GetComponent<TerrainCollider> ().terrainData;
		worldSize = new Vector3 (100, 100, 100);
		myTerrainData.size = worldSize;
		myTerrainData.heightmapResolution = resolution;
		heightArray = new float[resolution, resolution];

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            PotatoChip();  //aka hyperbolic paraboloid
        }
        else
        {
            Perlin();
        }

		// Assign values from heightArray into the terrain object's heightmap
		myTerrainData.SetHeights(0, 0, heightArray);

        mainCamera?.transform.Translate(new Vector3(0, myTerrainData.size.y - 10, 0));
	}
	

	void Update () 
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(1);
        }

        mainCamera?.transform.LookAt(vehicle?.transform);
    }

    private void OnGUI()
    {
        GUI.skin.box.wordWrap = true;

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            GUI.Box
                (
                    new Rect(10, 10, 150, 100),
                    string.Format("Current Scene\n1: Potato Chip\n\nPress 1 or 2 to swap to that scene.")
                );
        }
        else
        {
            GUI.Box
                (
                    new Rect(10, 10, 150, 100),
                    string.Format("Current Scene\n2: Perlin\n\nPress 1 or 2 to swap to that scene.")
                );
        }
    }


    void PotatoChip()  //generate a surface known as a hyperbolic paraboloid
    {
        // Fill heightArray with values
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                // Must cast either j or resolution to a float, otherwise integer division occurs
                heightArray[i, j] = 0.25f + (float)(i-(resolution-1)/2)/(float)(resolution-1) * (float)(j-(resolution-1)/2)/(float)(resolution-1) ;
            }
        }

    }

    /// <summary>
    /// Perlin()
    /// Assigns heightArray values using Perlin noise
    /// </summary>
    void Perlin()
	{

        // Perlin noise value - this is the starting "index" where the PerlinNoise function

        float originate = Random.Range(0.0f, 100.0f);

        float xIndex = originate; 
        float yIndex = originate; 

            // Fill heightArray with Perlin generated values

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                 
                    yIndex += 0.020f;
                    heightArray[i, j] = Mathf.PerlinNoise(xIndex, yIndex);
                }

     
            yIndex = originate;

                xIndex += .020f;
            }
     
    }
}
