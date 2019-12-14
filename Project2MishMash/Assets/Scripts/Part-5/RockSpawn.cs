using UnityEngine;

public class RockSpawn : MonoBehaviour
{
	public GameObject rock;
	public float minSpawnDelay = 1f;
	public float maxSpawnDelay = 3f;
	public float spawnXLimit = 6f;
    public GameManager gameManager; 

	void Start()
	{
		Spawn();
	}

	void Spawn()
	{
		// Create a meteor at a random x position
		float random = Random.Range(-spawnXLimit, spawnXLimit);
		Vector3 spawnPos = transform.position + new Vector3(random, 0f, 0f);
		gameManager.AddRockToList(Instantiate(rock, spawnPos, Quaternion.identity));

		Invoke("Spawn", Random.Range(minSpawnDelay, maxSpawnDelay));
	}
}
