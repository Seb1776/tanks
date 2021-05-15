using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Visible
    public enum BulletType {Normal, Sniper, Fire, Explosive, Piercing, Tasing}
    public BulletType currentType;
    public float speed;
    public float lifeTime;
    public float impactForce;
    public float damage;
    public float decreaseFactor;
    [Header("Fire Bullet Properties")]
    public GameObject fireEffect;
    [Header("Tasing Bullet Properties")]
    public GameObject electricEffect;
    [Header("Explosion Bullet Properties")]
    public GameObject explosionEffect;
    public float explosionRadius;
    public float closeExplosionDamage;
    public float closeExplosionImpactForce;
    [Header("Fire/Explosion Bullet Properties")]
    public float damagePerSecond;
    public float fireDamageDuration;
    [Header("Piercing Bullet Properties")]
    public int piercableEnemiesUntilDeath;

    //Invisible
    int piercedEnemies;
    float currentLifeTime;
    bool destroying;
    bool fire;
    float currentFireLifeTime;
    float currentFireDamageDuration;
    Tank player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Tank>();
        currentLifeTime = lifeTime;
        currentFireDamageDuration = fireDamageDuration;
    }

    void Update()
    {   
        if (!destroying)
            if (currentLifeTime <= 0)
                DestroyBullet();
            else
                currentLifeTime -= Time.deltaTime;
        else
            DestroyBullet();
    }
    
    void FixedUpdate()
    {
        if (!destroying)
            transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

    void DestroyBullet()
    {
        Vector2 tmpLocalScale = this.transform.localScale;
        this.GetComponent<CircleCollider2D>().enabled = false;

        if (tmpLocalScale.x > 0 && tmpLocalScale.y > 0)
        {
            tmpLocalScale.x -= Time.deltaTime * decreaseFactor;
            tmpLocalScale.y -= Time.deltaTime * decreaseFactor;
        }

        else
            Destroy(this.gameObject);

        this.transform.localScale = tmpLocalScale;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.transform.CompareTag("Enemy") && !this.gameObject.CompareTag("EnemyBullet"))
        {
            other.transform.GetComponent<Rigidbody2D>().AddForce(transform.right * impactForce);
            other.transform.GetComponent<EnemyTank>().MakeDamage(damage);
            player.Heal(damage / 6);

            switch (currentType)
            {
                case BulletType.Explosive:
                    GameObject prefBullet = Instantiate(explosionEffect, transform);
                    prefBullet.transform.parent = null;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

                    foreach (Collider2D affected in colliders)
                    {
                        if (affected.GetComponent<EnemyTank>() != null)
                        {
                            affected.GetComponent<EnemyTank>().MakeDamage(closeExplosionDamage);
                            affected.GetComponent<Rigidbody2D>().AddForce(-transform.right * closeExplosionImpactForce);
                        }
                    }

                    destroying = true;
                break;

                case BulletType.Fire:
                    other.transform.GetComponent<EnemyTank>().CheckForFire(fireEffect);
                    destroying = true;
                break;

                case BulletType.Tasing:
                    other.transform.GetComponent<EnemyTank>().CheckForElectricity(electricEffect);
                    destroying = true;
                break;

                case BulletType.Piercing:
                    if (piercedEnemies >= piercableEnemiesUntilDeath)
                        destroying = true;
                    else
                        piercedEnemies++;
                break;

                default:
                    destroying = true;
                break;
            }
        }

        else if (other.transform.CompareTag("Player") && !this.gameObject.CompareTag("Bullet"))
        {
            other.transform.GetComponent<Rigidbody2D>().AddForce(transform.right * impactForce);
            other.transform.GetComponent<Tank>().MakeDamage(damage);

            switch (currentType)
            {
                case BulletType.Explosive:
                    GameObject prefBullet = Instantiate(explosionEffect, transform);
                    prefBullet.transform.parent = null;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

                    foreach (Collider2D affected in colliders)
                    {
                        if (affected.GetComponent<Tank>() != null)
                        {
                            affected.GetComponent<Tank>().MakeDamage(closeExplosionDamage);
                            affected.GetComponent<Rigidbody2D>().AddForce(-transform.right * closeExplosionImpactForce);
                        }
                    }

                    destroying = true;
                break;

                case BulletType.Fire:
                    other.transform.GetComponent<Tank>().CheckForFire(fireEffect);
                    destroying = true;
                break;

                case BulletType.Tasing:
                    other.transform.GetComponent<Tank>().CheckForElectricity(electricEffect);
                    destroying = true;
                break;

                case BulletType.Piercing:
                    if (piercedEnemies >= piercableEnemiesUntilDeath)
                        destroying = true;
                    else
                        piercedEnemies++;
                break;

                default:
                    destroying = true;
                break;
            }
        }

        if (other.transform.CompareTag("Shield") && !this.CompareTag("EnemyBullet"))
            destroying = true;
    }

    void OnDrawGizmos()
    {   
        if (currentType == BulletType.Explosive)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
