using UnityEngine;

public class ShipControl : MonoBehaviour
{
	public GameManager gameManager;
	public GameObject antiRock;
	public float speed = 10f;
	public float xLimit = 7;
	public float reloadTime = 0.5f; 

	float elapsedTime = 0;

	void Update()
	{
        // Keeping track of time for bullet firing
        elapsedTime += Time.deltaTime;

        //clamp the ship's x position
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -xLimit, xLimit);
        transform.position = position;

        // --- Manual Control --- \\
        //// Keeping track of time for bullet firing
        //elapsedTime += Time.deltaTime;

        //// Move the player left and right
        //float xInput = Input.GetAxis("Horizontal");
        //transform.Translate(xInput * speed * Time.deltaTime, 0f, 0f);

        ////clamp the ship's x position
        //Vector3 position = transform.position;
        //position.x = Mathf.Clamp(position.x, -xLimit, xLimit);
        //transform.position = position;

        //// Spacebar fires. The default InputManager settings call this “Jump”
        //// Only happens if enough time has elapsed since last firing.
        //      //Also doesn't happen if currently game over'd. (Otherwise, player can destroy the rock that killed them by pressing space)
        //if (Input.GetButtonDown("Jump") && elapsedTime > reloadTime && !gameManager.gameOverText.enabled)
        //{
        //	// Instantiate the bullet 1.2 units in front of the player
        //	Vector3 spawnPos = transform.position;
        //	spawnPos += new Vector3(0, 1.2f, 0);
        //          gameManager.AddAntiRockToList(Instantiate(antiRock, spawnPos, Quaternion.identity));

        //          elapsedTime = 0f; // Reset bullet firing timer
        //}
    }

}
