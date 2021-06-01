using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Visible
    public enum BulletType {Normal, Sniper, Fire, Explosive, Piercing, Tasing, Smoke}
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
    [Header("Smoke Bullet Properties")]
    public float smokeDuration;
    public float smokeRadius;
    public float dmgPerSecond;
    public GameObject smokeEffect;
    public GameObject skin;
    public GameObject skinOut;

    //Invisible
    int piercedEnemies;
    float currentLifeTime;
    bool destroying;
    bool fire;
    bool createdEffect;
    bool destroyedTrail;
    float currentFireLifeTime;
    float currentSmokeTimeBtwDamages;
    float currentFireDamageDuration;
    float currentSmokeDuration;
    Tank player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Tank>();
        currentLifeTime = lifeTime;
        currentFireDamageDuration = fireDamageDuration;
        currentSmokeDuration = smokeDuration;
    }

    void Update()
    {   
        if (!destroying)
            if (currentLifeTime <= 0)
                DestroyBullet(false);
            else
                currentLifeTime -= Time.deltaTime;
        else
            if (currentType == BulletType.Smoke)
                DestroyBullet(true);
            else
                DestroyBullet(false);
    }
    
    void FixedUpdate()
    {
        if (!destroying)
            transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

    IEnumerator DestroyEffect(float delay, ParticleSystem effect)
    {
        yield return new WaitForSeconds(delay);
        effect.Stop();
    }

    void DestroyBullet(bool onContact)
    {
        if (!destroyedTrail)
        {
            Destroy(gameObject.transform.GetChild(0).transform.GetChild(2).gameObject);
            destroyedTrail = true;
        }

        if (currentType == BulletType.Smoke || onContact)
        {
            skin.SetActive(false);
            skinOut.SetActive(false);
            GetComponent<Collider2D>().enabled = false;

            if (!createdEffect)
            {
                GameObject tmp = Instantiate(smokeEffect, transform);
                StartCoroutine(DestroyEffect(smokeDuration - .5f, tmp.GetComponent<ParticleSystem>()));
                createdEffect = true;
            }

            if (currentSmokeDuration <= 0f)
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

            else
            {
                if (currentSmokeTimeBtwDamages <= 0f)
                {
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, smokeRadius);

                    foreach(Collider2D near in colliders)
                    {
                        if (near.GetComponent<EnemyTank>() != null)
                            near.GetComponent<EnemyTank>().MakeDamage(dmgPerSecond);
                        
                        if (near.GetComponent<Tank>() != null)
                            near.GetComponent<Tank>().MakeDamage(dmgPerSecond);
                    }

                    currentSmokeTimeBtwDamages = 1f;
                }

                else
                    currentSmokeTimeBtwDamages -= Time.deltaTime;

                currentSmokeDuration -= Time.deltaTime;
            }
        }

        else if (currentType != BulletType.Smoke || !onContact)
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
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.transform.CompareTag("Enemy") && !this.gameObject.CompareTag("EnemyBullet"))
        {
            other.transform.GetComponent<Rigidbody2D>().AddForce(transform.right * impactForce);
            other.transform.GetComponent<EnemyTank>().MakeDamage(damage);

            if (player != null)
                player.Heal(damage / 2.5f);

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

        if ((other.transform.CompareTag("Shield") && !this.CompareTag("EnemyBullet")) || other.transform.CompareTag("Collisionable"))
            destroying = true;
        
        if (other.transform.CompareTag("Shape") && !this.CompareTag("EnemyBullet"))
        {
            other.GetComponent<Shape>().MakeDamage(damage);
            other.GetComponent<Rigidbody2D>().AddForce(transform.right * impactForce);
            destroying = true;
        }
    }

    void OnDrawGizmos()
    {   
        if (currentType == BulletType.Explosive)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }

        if (currentType == BulletType.Smoke)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, smokeRadius);
        }
    }
}
