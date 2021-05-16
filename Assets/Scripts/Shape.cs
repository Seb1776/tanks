using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    //Visible
    public float health;
    public int score;
    public float healAmount;
    public float spawnSpace;
    public float rotationSpeed;
    [Range (0.1f, 0.8f)]
    public float probabilityOfSpawn;
    public GameObject skin;
    public bool showBounds;


    //Invisible
    bool safe;
    float currentHealth;
    GameManager gameManager;
    Collider2D collider;

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        collider = GetComponent<Collider2D>();
        skin.SetActive(false);
        collider.enabled = false;
    }

    void Start()
    {
        currentHealth = health;

        if (!safe)
        {
            if (CheckForSpawn())
            {
                skin.SetActive(true);
                collider.enabled = true;
                safe = true;
            }
        }
    }

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    bool CheckForSpawn()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, spawnSpace);

        foreach (Collider2D near in colliders)
        {
            if (near.GetComponent<Shape>() != null && (near.GetComponent<Tank>() != null || near.GetComponent<EnemyTank>() != null) && near.gameObject != this.gameObject)
            {
                gameManager.DestroyShape(this.gameObject, 0);
                return false;
            }
        }

        if (Random.value < probabilityOfSpawn)
        {
            gameManager.DestroyShape(this.gameObject, 0);
            return false;
        }

        return true;
    }

    public void ExternDestroy(GameObject shape)
    {
        gameManager.DestroyShape(shape, score);
    }

    public void MakeDamage(float amount)
    {
        if (currentHealth <= 0)
        {
            collider.enabled = false;
            gameManager.DestroyShape(this.gameObject, score);
        }
            
        else
            currentHealth -= amount;
    }

    void OnDrawGizmos()
    {
        if (showBounds)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnSpace);
        }
    }
}
