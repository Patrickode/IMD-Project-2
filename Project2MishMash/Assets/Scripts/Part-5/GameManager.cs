using UnityEngine;
using UnityEngine.UI; // Note this new line is needed for UI
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public Text scoreText;
	public Text gameOverText;
    CollisionDetector collisionDetector;
    public List<GameObject> rocks;
    public List<GameObject> antirocks;
    public GameObject rocket;
    GameObject antirock;
    GameObject rock;

	int playerScore = 0;

    bool rockOut;
    bool shouldDisposeRock;
    bool shouldDisposeAntiRock;

    public void Start()
    {
        collisionDetector = gameObject.GetComponent<CollisionDetector>();
        rocks = new List<GameObject>();
        antirocks = new List<GameObject>();

        Time.timeScale = 1;
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //check to see whether any rock has rolled onto the rocket, or if any are offscreen
        foreach (GameObject rck in rocks)
        {
            if (collisionDetector.AABBTest(rocket, rck))
            {
                PlayerDied();
            }
        }

        //For all bullets currently existing, check if they're colliding with any rocks
        rockOut = false;
        foreach (GameObject antirck in antirocks )
        {
            foreach (GameObject rck in rocks)
            {
                if (collisionDetector.AABBTest(antirck, rck))
                {
                    //Debug.Log("antirock hit rock!");
                    rockOut = true;
                    antirock = antirck;
                    rock = rck;
                    break;
                }
            }

            if (rockOut)
            {
                break;
            }
        }

        //If a bullet and rock collided, destroy them and add to the score.
        if (rockOut)
        {
            DisposeObject(rock);
            DisposeObject(antirock);
            AddScore();
        }

        //If the player presses R when dead, allow them to restart.
        if (Input.GetKeyDown(KeyCode.R) && gameOverText.enabled)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    //There are separate public functions so other scripts can use them.
    public void AddRockToList(GameObject rock)
    {
        rocks.Add(rock);
    }

    public bool RemoveRockFromList(GameObject rock)
    {
        return rocks.Remove(rock);
    }

    public void AddAntiRockToList(GameObject antirock)
    {
        antirocks.Add(antirock);
    }

    public bool RemoveAntiRockFromList(GameObject antirock)
    {
        return antirocks.Remove(antirock);
    }

    public void DisposeObject(GameObject thing)
    {
        if (thing.CompareTag("Rock"))
        {
            RemoveRockFromList(thing);
            Destroy(thing);
        }
        else if (thing.CompareTag("AntiRock"))
        {
            RemoveAntiRockFromList(thing);
            Destroy(thing);
        }
        else
        {
            Debug.LogWarning("Attempted to dispose object that was not a rock or antirock.");
        }
    }


    public void AddScore()
	{
		playerScore++;
		//This converts the score (a number) into a string
		scoreText.text = playerScore.ToString();
	}

	public void PlayerDied()
	{
		gameOverText.enabled = true;

		// This freezes the game
		Time.timeScale = 0;
	}
}
