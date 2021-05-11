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


    void Start()
    {
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
        if (other.transform.CompareTag("Enemy"))
        {
            if (currentType == BulletType.Explosive)
                Instantiate(explosionEffect, transform);

            other.transform.GetComponent<Rigidbody2D>().AddForce(transform.right * impactForce);
            other.transform.GetComponent<Enemy>().MakeDamage(damage);

            if (currentType == BulletType.Fire)
                other.transform.GetComponent<Enemy>().CheckForFire(fireEffect);
            
            if (currentType == BulletType.Tasing)
                other.transform.GetComponent<Enemy>().CheckForElectricity(electricEffect);

            if (currentType != BulletType.Piercing)        
                destroying = true;

            else
            {
                if (piercedEnemies >= piercableEnemiesUntilDeath)
                    destroying = true;
                else
                    piercedEnemies++;
            }
        }

        else if (other.transform.CompareTag("Shield"))
            destroying = true;
    }
}
