using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	//Visible
	public bool canSpawn;
	public Vector2 randTimeBtwSpawns;
	public int enemySpawnLimit;
	public List<GameObject> enemies = new List<GameObject>();
	public GameObject turretEnemy;
	public Transform[] turretPositions;
	public float timeToTurret;
	public float chanceToTurret;
	public bool spawnedTurret;
	public bool lightHeavyEnemy;
	public bool shieldEnemy;
	public bool sniperEnemy;
	public bool taserEnemy;
	public bool medicEnemy;
	public bool smokerEnemy;
	public bool bulldozerLight;
	public bool bulldozerMedium;
	public bool bulldozerHeavy;
	public bool turret;



	//Invisible
	GameManager gameManager;
	float timeBtwSpawns;
	float currentTimeBtwSpawns;
	float currentTimeToTurret;
	int randIndex;
	int limit;


	void Start()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		currentTimeToTurret = timeToTurret;

		CountLimit();
		ResetTimers();
	}

	void Update()
	{
		if (canSpawn)
		{
			HandleSpawners();
		
			if (turret && !spawnedTurret)
			{
				if (currentTimeToTurret <= 0)
				{	
					if (Random.value > chanceToTurret)
					{
						Instantiate(turretEnemy, turretPositions[Random.Range(0, turretPositions.Length)].position, turretPositions[Random.Range(0, turretPositions.Length)].rotation);
						spawnedTurret = true;
					}

					currentTimeToTurret = timeToTurret;
				}

				else
					currentTimeToTurret -= Time.deltaTime;
			}
		}
	}

	public void ResetTimers() => timeBtwSpawns = Random.Range(randTimeBtwSpawns.x, randTimeBtwSpawns.y);

	public void CountLimit()
	{
		limit = 0;

		if (lightHeavyEnemy)
			limit += 2;
		if (shieldEnemy)
			limit++;
		if (sniperEnemy)
			limit++;
		if (taserEnemy)
			limit++;
		if (medicEnemy)
			limit++;
		if (smokerEnemy)
			limit++;
		if (bulldozerLight)
			limit += 3;
		if (bulldozerMedium)
			limit += 2;
		if (bulldozerHeavy)
			limit += 4;
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

			if (randIndex >= 0 && randIndex <= 1)
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

			else if (randIndex >= 3 && randIndex <= 4)
			{
				if (randIndex == 3)
				{
					if (gameManager.currentLimitSniperEnemy < gameManager.limitSniperEnemy)
					{
						for (int j = 0; j < enemySpawnLimit; j++)
						{
							Instantiate(enemies[3], transform.position, transform.rotation);
							gameManager.currentLimitSniperEnemy++;
						}
					}
				}

				else if (randIndex == 4)
				{
					if (gameManager.currentLimitTaserEnemy < gameManager.limitTaserEnemy)
					{
						for (int j = 0; j < enemySpawnLimit; j++)
						{
							Instantiate(enemies[4], transform.position, transform.rotation);
							gameManager.currentLimitTaserEnemy++;
						}
					}
				}
			}

			else if (randIndex == 5)
			{
				if (gameManager.currentLimitMedicEnemy < gameManager.limitMedicEnemy)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						Instantiate(enemies[5], transform.position, transform.rotation);
						gameManager.currentLimitMedicEnemy++;
					}
				}
			}

			else if (randIndex == 6)
			{
				if (gameManager.currentLimitSmokerEnemy < gameManager.limitSmokerEnemy)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						Instantiate(enemies[6], transform.position, transform.rotation);
						gameManager.currentLimitSmokerEnemy++;
					}
				}
			}

			else if (randIndex >= 7 && randIndex <= 9)
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

			else if (randIndex >= 10 && randIndex <= 11)
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

			else if (randIndex >= 12 && randIndex <= 15)
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
