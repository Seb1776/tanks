using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    //Visible
    public List<EnemyTank> lightHeavyEnemies = new List<EnemyTank>();


    //Invisible
    GameManager gameManager;


    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManagers").GetComponent<GameManager>();
    }

    void SendTanksToSpawn()
    {

    }
}
