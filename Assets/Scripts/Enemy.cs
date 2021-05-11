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
    public float electricEffectDuration;
    public Color tankColor;
    public SpriteRenderer tankSkin;
    public SpriteRenderer outline;
    public ParticleSystem dieEffect;


    //Invisible
    [HideInInspector]
    public bool fire;
    bool stun;
    bool electrify;
    bool lookAtPlayer = true;
    List<GameObject> instEffect = new List<GameObject>();
    float currentFireLifeTime;
    float currentFireDamageDuration;
    float currentElectricEffectDuration;
    float currentElectricReload;
    bool dead;
    bool generatedEffect;
    GameObject player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentFireLifeTime = 1;
        currentFireDamageDuration = fireDamageDuration;
        currentElectricEffectDuration = electricEffectDuration;

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
        if (lookAtPlayer)
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
        this.GetComponent<Collider2D>().enabled = false;

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
                foreach (GameObject inst in instEffect)
                    inst.transform.parent = null;

                for (int i = 0; i < instEffect.Count; i++)
                {
                    if (instEffect[i].name == "FireEffect")
                    {
                        GameObject tmpF = instEffect[i];
                        StartCoroutine(DestroyEffectAferTime(tmpF));
                        break;
                    }
                }
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
        GameObject tmpEffect = Instantiate(effect, this.transform);
        instEffect.Add(tmpEffect);
        tmpEffect.transform.parent = this.transform;
        tmpEffect.transform.localPosition = new Vector2(0f, 0f);
    }

    public void ElectricEffect(GameObject effect)
    {
        GameObject tmpEffect = Instantiate(effect, this.transform);
        instEffect.Add(tmpEffect);
        tmpEffect.transform.parent = this.transform;
        tmpEffect.transform.localPosition = new Vector2(0f, 0f);
    }

    void SetOnFire()
    {
        if (currentFireDamageDuration <= 0)
        {
            fire = false;
            currentFireDamageDuration = fireDamageDuration;

            for (int i = 0; i < instEffect.Count; i++)
            {
                if (instEffect[i].name == "FireEffect")
                {
                    GameObject tmpF = instEffect[i];
                    StartCoroutine(DestroyEffectAferTime(tmpF));
                    break;
                }
            }
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

        if (!instEffect.Contains(effect))
            FireEffect(effect);
    }

    public void CheckForElectricity(GameObject effect)
    {
        if (!electrify && !stun)
            if(CheckForRandomElectric())
            {
                SetElectricEffect();

                if (!instEffect.Contains(effect))
                    ElectricEffect(effect);
            }
        else
            currentElectricEffectDuration = electricEffectDuration;
    }

    bool CheckForRandomElectric()
    {
        return (Random.value > 0.5);
    }

    void Electrified()
    {
        if (currentElectricEffectDuration < 0)
        {
            electrify = false;
            //lookAtPlayer = true;
            currentElectricEffectDuration = electricEffectDuration;

            for (int i = 0; i < instEffect.Count; i++)
            {
                if (instEffect[i].name == "ElectricEffect")
                {
                    GameObject tmpE = instEffect[i];
                    StartCoroutine(DestroyEffectAferTime(tmpE));
                    break;
                }
            }
        }

        else
        {
            lookAtPlayer = false;

            if (currentElectricReload < 0)
            {
                var thisRotation = transform.eulerAngles;
                thisRotation.z = Random.Range(0f, 360f);
                transform.eulerAngles = thisRotation;
                currentElectricReload = 1f;
            }

            else
                currentElectricReload -= Time.deltaTime;
            
            currentElectricEffectDuration -= Time.deltaTime;
        }
    }

    void SetElectricEffect()
    {
        if (Random.value > 0.5)
            electrify = true;
        else
            electrify = true;
    }

    void DestroyEffect(string effect)
    {
        GameObject tmpF = null;
        GameObject tmpE = null;

        switch(effect)
        {
            case "Fire":
                for (int i = 0; i < instEffect.Count; i++)
                {
                    if (instEffect[i].name == "FireEffect")
                    {
                        tmpF = instEffect[i];
                        StartCoroutine(DestroyEffectAferTime(tmpF));
                        break;
                    }
                }
            break;

            case "Electric":
                for (int i = 0; i < instEffect.Count; i++)
                {
                    if (instEffect[i].name == "FireEffect")
                    {
                        tmpE = instEffect[i];
                        StartCoroutine(DestroyEffectAferTime(tmpE));
                        break;
                    }
                }
            break;
        }
    }

    IEnumerator DestroyEffectAferTime(GameObject effectTo)
    {
        effectTo.GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1f);
        Destroy(effectTo);
    }
}
