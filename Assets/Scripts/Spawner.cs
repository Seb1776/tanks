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
	public float timeBtwSpawns;
	public float currentTimeBtwSpawns;
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

	public void ResetTimers()
	{
		timeBtwSpawns = Random.Range(randTimeBtwSpawns.x, randTimeBtwSpawns.y);
		gameManager.originalSpawnTimes.Add(timeBtwSpawns);
	}

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
			GameObject tmpEne = null;

			randIndex = Random.Range(0, limit);

			if (randIndex >= 0 && randIndex <= 1)
			{
				if (gameManager.currentLimitLightHeavyEnemy < gameManager.limitLightHeavyEnemy)
				{
					for (int j = 0; j < enemySpawnLimit; j++)
					{
						tmpEne = Instantiate(enemies[randIndex], transform.position, transform.rotation);

						foreach (Flank flanks in tmpEne.GetComponent<EnemyTank>().tankFlanks)
						{
							flanks.modifyiedValue = AssignBalancedModifyier(gameManager.currentDifficulty.ToString());
							flanks.value = AssignBalancedDamageValue(gameManager.currentDifficulty.ToString());
						}

						tmpEne.GetComponent<EnemyTank>().health = AssignBalancedHealth(tmpEne.GetComponent<EnemyTank>().health, gameManager.currentDifficulty.ToString());

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
						tmpEne = Instantiate(enemies[randIndex], transform.position, transform.rotation);

						foreach (Flank flanks in tmpEne.GetComponent<EnemyTank>().tankFlanks)
						{
							flanks.modifyiedValue = AssignBalancedModifyier(gameManager.currentDifficulty.ToString());
							flanks.value = AssignBalancedDamageValue(gameManager.currentDifficulty.ToString());
						}

						tmpEne.GetComponent<EnemyTank>().health = AssignBalancedHealth(tmpEne.GetComponent<EnemyTank>().health, gameManager.currentDifficulty.ToString());

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
							tmpEne = Instantiate(enemies[randIndex], transform.position, transform.rotation);

							foreach (Flank flanks in tmpEne.GetComponent<EnemyTank>().tankFlanks)
							{
								flanks.modifyiedValue = AssignBalancedModifyier(gameManager.currentDifficulty.ToString());
								flanks.value = AssignBalancedDamageValue(gameManager.currentDifficulty.ToString());
							}

							tmpEne.GetComponent<EnemyTank>().health = AssignBalancedHealth(tmpEne.GetComponent<EnemyTank>().health, gameManager.currentDifficulty.ToString());
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
							tmpEne = Instantiate(enemies[randIndex], transform.position, transform.rotation);

							foreach (Flank flanks in tmpEne.GetComponent<EnemyTank>().tankFlanks)
							{
								flanks.modifyiedValue = AssignBalancedModifyier(gameManager.currentDifficulty.ToString());
								flanks.value = AssignBalancedDamageValue(gameManager.currentDifficulty.ToString());
							}

							tmpEne.GetComponent<EnemyTank>().health = AssignBalancedHealth(tmpEne.GetComponent<EnemyTank>().health, gameManager.currentDifficulty.ToString());

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
						tmpEne = Instantiate(enemies[randIndex], transform.position, transform.rotation);

						foreach (Flank flanks in tmpEne.GetComponent<EnemyTank>().tankFlanks)
						{
							flanks.modifyiedValue = AssignBalancedModifyier(gameManager.currentDifficulty.ToString());
							flanks.value = AssignBalancedDamageValue(gameManager.currentDifficulty.ToString());
						}

						tmpEne.GetComponent<EnemyTank>().health = AssignBalancedHealth(tmpEne.GetComponent<EnemyTank>().health, gameManager.currentDifficulty.ToString());
						
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
						tmpEne = Instantiate(enemies[randIndex], transform.position, transform.rotation);

						foreach (Flank flanks in tmpEne.GetComponent<EnemyTank>().tankFlanks)
						{
							flanks.modifyiedValue = AssignBalancedModifyier(gameManager.currentDifficulty.ToString());
							flanks.value = AssignBalancedDamageValue(gameManager.currentDifficulty.ToString());
						}

						tmpEne.GetComponent<EnemyTank>().health = AssignBalancedHealth(tmpEne.GetComponent<EnemyTank>().health, gameManager.currentDifficulty.ToString());
						
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
						tmpEne = Instantiate(enemies[randIndex], transform.position, transform.rotation);

						foreach (Flank flanks in tmpEne.GetComponent<EnemyTank>().tankFlanks)
						{
							flanks.modifyiedValue = AssignBalancedModifyier(gameManager.currentDifficulty.ToString());
							flanks.value = AssignBalancedDamageValue(gameManager.currentDifficulty.ToString());
						}

						tmpEne.GetComponent<EnemyTank>().health = AssignBalancedHealth(tmpEne.GetComponent<EnemyTank>().health, gameManager.currentDifficulty.ToString());
						
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
						tmpEne = Instantiate(enemies[randIndex], transform.position, transform.rotation);

						foreach (Flank flanks in tmpEne.GetComponent<EnemyTank>().tankFlanks)
						{
							flanks.modifyiedValue = AssignBalancedModifyier(gameManager.currentDifficulty.ToString());
							flanks.value = AssignBalancedDamageValue(gameManager.currentDifficulty.ToString());
						}

						tmpEne.GetComponent<EnemyTank>().health = AssignBalancedHealth(tmpEne.GetComponent<EnemyTank>().health, gameManager.currentDifficulty.ToString());
						
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
						tmpEne = Instantiate(enemies[randIndex], transform.position, transform.rotation);

						foreach (Flank flanks in tmpEne.GetComponent<EnemyTank>().tankFlanks)
						{
							flanks.modifyiedValue = AssignBalancedModifyier(gameManager.currentDifficulty.ToString());
							flanks.value = AssignBalancedDamageValue(gameManager.currentDifficulty.ToString());
						}

						tmpEne.GetComponent<EnemyTank>().health = AssignBalancedHealth(tmpEne.GetComponent<EnemyTank>().health, gameManager.currentDifficulty.ToString());
						
						gameManager.currentLimitBulldozerHeavy++;
					}
				}
			}
		}
	}

	bool AssignBalancedModifyier(string difficulty)
	{
		switch (difficulty)
		{
			case "Normal": case "Hard": case "VeryHard":
			case "Overkill": case "Mayhem": case "DeathWish":
			case "DeathSentence":
				return true;
			
			case "OneDown":
				return false;
		}

		return false;
	}

	float AssignBalancedDamageValue(string difficulty)
	{
		float difficultyDivider = 1f;

		switch (difficulty)
		{
			case "Normal":
				difficultyDivider = 7f;
			break;

			case "Hard":
				difficultyDivider = 6f;
			break;

			case "VeryHard":
				difficultyDivider = 5f;
			break;

			case "Overkill":
				difficultyDivider = 4f;
			break;

			case "Mayhem":
				difficultyDivider = 3f;
			break;

			case "DeathWish":
				difficultyDivider = 2f;
			break;

			case "Death Sentence":
				difficultyDivider = 1f;
			break;
		}

		return difficultyDivider;
	}

	float AssignBalancedHealth(float healthValue, string difficulty)
	{
		float difficultyDivider = 1f;

		switch (difficulty)
		{
			case "Normal":
				difficultyDivider = 7f;
			break;

			case "Hard":
				difficultyDivider = 6f;
			break;

			case "VeryHard":
				difficultyDivider = 5f;
			break;

			case "Overkill":
				difficultyDivider = 4f;
			break;

			case "Mayhem":
				difficultyDivider = 3f;
			break;

			case "DeathWish":
				difficultyDivider = 2f;
			break;

			case "DeathSentence":
				difficultyDivider = 1f;
			break;
		}

		return healthValue / difficultyDivider;
	}
}
