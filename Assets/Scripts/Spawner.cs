using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	//Visible
	public bool canSpawn;
	public float timeBtwSpawns;
	public int enemySpawnLimit;
	public List<GameObject> enemies = new List<GameObject>();
	public bool lightHeavyEnemy;
	public bool shieldEnemy;
	public bool taserEnemy;
	public bool medicEnemy;
	public bool bulldozerLight;
	public bool bulldozerMedium;
	public bool bulldozerHeavy;



	//Invisible
	GameManager gameManager;
	float currentTimeBtwSpawns;
	int randIndex;
	int limit;


	void Start()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		if (lightHeavyEnemy)
			limit += 2;
		if (shieldEnemy)
			limit++;
		if (taserEnemy)
			limit++;
		if (medicEnemy)
			limit++;
		if (bulldozerLight)
			limit += 3;
		if (bulldozerMedium)
			limit += 2;
		if (bulldozerHeavy)
			limit += 3;
	}

	void Update()
	{
		if (canSpawn)
			HandleSpawners();
	}

	public void HandleSpawners()
	{
		if (currentTimeBtwSpawns <= 0)
		{
			SpawnEnemy();
			currentTimeBtwSpawns = timeBtwSpawns;
		}

		else
			currentTimeBtwSpawns -= Time.deltaTime;
	}

	public void SpawnEnemy()
	{
		if (enemies.Count != 0 || enemies != null)
		{
			randIndex = Random.Range(0, limit);

			if (randIndex >= 0 && randIndex <= 2)
			{
				if (gameManager.currentLimitLightHeavyEnemy < gameManager.limitLightHeavyEnemy)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						Instantiate(enemies[randIndex], transform.position, transform.rotation);
						gameManager.currentLimitLightHeavyEnemy++;
					}
				}
			}

			else if (randIndex == 2)
			{
				if (gameManager.currentLimitShieldEnemy < gameManager.limitShieldEnemy)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						Instantiate(enemies[2], transform.position, transform.rotation);
						gameManager.currentLimitShieldEnemy++;
					}
				}
			}

			else if (randIndex == 3)
			{
				if (gameManager.currentLimitTaserEnemy < gameManager.limitTaserEnemy)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						Instantiate(enemies[3], transform.position, transform.rotation);
						gameManager.currentLimitTaserEnemy++;
					}
				}
			}

			else if (randIndex == 4)
			{
				if (gameManager.currentLimitMedicEnemy < gameManager.limitMedicEnemy)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						Instantiate(enemies[4], transform.position, transform.rotation);
						gameManager.currentLimitMedicEnemy++;
					}
				}
			}

			else if (randIndex >= 5 && randIndex <= 7)
			{
				if (gameManager.currentLimitBulldozerLight < gameManager.limitBulldozerLight)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						Instantiate(enemies[randIndex], transform.position, transform.rotation);
						gameManager.currentLimitBulldozerLight++;
					}
				}
			}

			else if (randIndex >= 8 && randIndex <= 9)
			{
				if (gameManager.currentLimitBulldozerMedium < gameManager.limitBulldozerMedium)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						Instantiate(enemies[randIndex], transform.position, transform.rotation);
						gameManager.currentLimitBulldozerMedium++;
					}
				}
			}

			else if (randIndex >= 10 && randIndex <= 12)
			{
				if (gameManager.currentLimitBulldozerHeavy < gameManager.limitBulldozerHeavy)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						Instantiate(enemies[randIndex], transform.position, transform.rotation);
						gameManager.currentLimitBulldozerHeavy++;
					}
				}
			}
		}
	}
}
