using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Visible
    public float health;
    public float moveSpeed;
    public Transform bulletPoint;
    public GameObject bullet;
    public float bulletSpeed;
    public float reloadTime;
    public float damagePerSecond;
    public float fireDamageDuration;
    public Color tankColor;
    public SpriteRenderer tankSkin;
    public SpriteRenderer outline;
    public ParticleSystem dieEffect;


    //Invisible
    [HideInInspector]
    public bool fire;
    GameObject instEffect;
    float currentFireLifeTime;
    float currentFireDamageDuration;
    bool dead;
    bool generatedEffect;
    GameObject player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentFireLifeTime = 1;
        currentFireDamageDuration = fireDamageDuration;

        SetTankColor();
    }

    void Update()
    {   
        if (!dead)
        {
            TankAI();

            if (fire)
                SetOnFire();
        }

        else
            NoMoreUgh();
    }

    void FixedUpdate()
    {
        
    }

    void TankAI()
    {
        transform.right = player.transform.position - transform.position;

        if (health <= 0)
            dead = true;
    }

    void SetTankColor()
    {
        tankSkin.color = tankColor;
        outline.color = Color.Lerp(tankColor, Color.black, .25f);
    }

    void Shoot()
    {   
        
    }

    void Reload()
    {
        
    }

    public void MakeDamage(float damage)
    {
        if (health > 1)
            health -= damage;

        else
        {
            dead = true;
        }
    }

    void NoMoreUgh()
    {
        Vector2 tmpScale = this.transform.localScale;
        this.GetComponent<CircleCollider2D>().enabled = false;

        if (tmpScale.x <= 0 && tmpScale.y <= 0)
        {
            if (!generatedEffect)
            {
                ParticleSystem tempEffect = Instantiate(dieEffect, transform.position, transform.rotation);
                tempEffect.startColor = tankColor;
                generatedEffect = true;
            }

            if (fire)
            {
                instEffect.transform.parent = null;
                StartCoroutine(DestroyEffect());
            }

            Destroy(this.gameObject);
        }

        else
        {
            tmpScale.x -= Time.deltaTime * 2;
            tmpScale.y -= Time.deltaTime * 2;
        }

        this.transform.localScale = tmpScale;
    }

    public void FireEffect(GameObject effect)
    {
        instEffect = Instantiate(effect, this.transform);
        instEffect.transform.parent = this.transform;
        instEffect.transform.localPosition = new Vector2(0f, 0f);
    }

    void SetOnFire()
    {
        if (currentFireDamageDuration <= 0)
        {
            fire = false;
            currentFireDamageDuration = fireDamageDuration;
            StartCoroutine(DestroyEffect());
        }

        else
        {
            if (currentFireLifeTime <= 0)
            {
                MakeDamage(damagePerSecond);
                currentFireLifeTime = 1;
            }

            else
            {
                currentFireLifeTime -= Time.deltaTime;
            }

            currentFireDamageDuration -= Time.deltaTime;
        }
    }

    public void CheckForFire(GameObject effect)
    {
        if (!fire)
            fire = true;
        else
            currentFireDamageDuration = fireDamageDuration;  

        if (instEffect == null)      
            FireEffect(effect);
    }

    IEnumerator DestroyEffect()
    {
        instEffect.GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1f);
        Destroy(instEffect);
    }
}
