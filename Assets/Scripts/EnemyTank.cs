using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    //Visible
    public enum EnemyFaction {LightHeavy, Shield, Taser, Medic, BulldozerLight, BulldozerMedium, BulldozerHeavy}
    public EnemyFaction currentRegion;
    public enum EnemyType {Normal, Special, Dozer, Turret}
    public EnemyType currentType;
    public enum EnemyState {Roam, Chase, Shoot, Stop, Retreat}
    public EnemyState currentState;
    public enum AttackMode {Spinfire, DirectHit}
    public AttackMode currentAttack;
    public float health;
    public float moveSpeed;
    public float attackDistance;
    public float recognizeDistance;
    public float stopDistance;
    public float rotationSpeed;
    public float lookSpeed;
    public Transform flankOrigin;
    public float damagePerSecond;
    public float fireDamageDuration;
    public float electricEffectDuration;
    public Color tankColor;
    public SpriteRenderer tankSkin;
    public SpriteRenderer outline;
    public ParticleSystem dieEffect;
    [Header("Medic / Medicdozer Properties")]
    public GameObject healEffect;
    public bool canHeal;
    public float timeBtwHeals;
    public float healAmount;
    public float healRadius;
    [Header("Sniper Properties")]
    public bool isSniper;
    public float sniperSightDistance;
    public float readyTimeToShot;
    public LineRenderer sniperLaser;
    public LayerMask playerMask;
    [Header("Shield Properties")]
    public bool isShield;
    public float timeLookingAt;
    [Header("Turret Properties")]
    public bool isTurret;
    public float shieldHealth;
    public int shieldDown;
    public float timeToRest;
    public bool resting;
    public ParticleSystem restingEffect;
    public ParticleSystem smokeExplosionEffect;
    [Header("Explosive Death Properties")]
    public bool canExplode;
    public float explosionRadius;
    public float damageExplosion;
    public float explosionForce;


    //Invisible
    public bool fire;
    [SerializeField]
    Vector2 minRoamingArea;
    [SerializeField]
    Vector2 maxRoamingArea;
    float currentHealth;
    bool stun;
    bool electrify;
    bool lookAtPlayer = true;
    List<GameObject> instEffect = new List<GameObject>();
    [SerializeField]
    List<Flank> tankFlanks = new List<Flank>();
    float currentFireLifeTime;
    float currentFireDamageDuration;
    float currentElectricEffectDuration;
    float timeBtwElectricEffect = 0.5f;
    float currentTimeBtwRoamPosition;
    float currentReadyTimeShot;
    float currentTimeBtwHeals;
    float currentTimeLookingAt;
    float currentSniperDistance;
    float currentShieldHealth;
    float currentTimeToRest;
    int currentShieldDown;
    bool dead;
    bool generatedEffect;
    bool generatedSmoke;
    bool createdPosition;
    bool retreating;
    GameObject smoke;
    GameObject player;
    GameManager gameManager;
    Vector3 roamingPosition;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        currentFireLifeTime = 1;
        currentFireDamageDuration = fireDamageDuration;
        currentElectricEffectDuration = electricEffectDuration;
        currentHealth = health;

        if (isSniper)
        {
            currentReadyTimeShot = readyTimeToShot;
            canHeal = false;
            isShield = false;
            isTurret = false;
        }

        else if (canHeal)
        {
            currentTimeBtwHeals = timeBtwHeals;
            isSniper = false;
            isShield = false;
            isTurret = false;
        }

        else if (isShield)
        {
            currentTimeLookingAt = timeLookingAt;
            canHeal = false;
            isSniper = false;
            isTurret = false;
        }

        else if (isTurret)
        {
            currentShieldHealth = shieldHealth;
            currentShieldDown = shieldDown;
            currentTimeToRest = timeToRest;
            canHeal = false;
            isShield = false;
            isSniper = false;
        }

        SetTankColor();
        SetTankFlanks();
    }

    void Update()
    {   
        if (!dead)
        {
            if (!isTurret)
                TankAI();
            else
                if (!resting)
                    TankAI();
                else
                    TurretRest();

            if (fire)
                SetOnFire();
            
            if (electrify && !stun)
                Electrified();

            if (!electrify && stun)
                Stun();
        }

        else
            NoMoreUgh();
    }

    void TankAI()
    {
        if (lookAtPlayer)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();
            float zAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion lookRot = Quaternion.Euler(0f, 0f, zAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, lookSpeed * Time.deltaTime);
        }

        if (CheckForDamageEffect())
        {
            if (retreating)
                currentState = EnemyState.Retreat;
            
            if (canHeal)
                MedicBehaviour();

            switch (currentState)
            {
                case EnemyState.Roam:
                    if (!createdPosition)
                    {
                        roamingPosition = new Vector3(Random.Range(minRoamingArea.x, maxRoamingArea.x), Random.Range(minRoamingArea.y, maxRoamingArea.y));
                        createdPosition = true;
                    }

                    if (this.transform.position != roamingPosition && createdPosition)
                        transform.position = Vector2.MoveTowards(transform.position, roamingPosition, moveSpeed * Time.deltaTime);
                    if (this.transform.position == roamingPosition)
                        createdPosition = false;
                    
                    lookAtPlayer = false;
                break;

                case EnemyState.Chase:
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                    lookAtPlayer = true;
                break;

                case EnemyState.Shoot:
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                    
                    switch(currentAttack)
                    {
                        case AttackMode.Spinfire:
                            lookAtPlayer = false;

                            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                        break;

                        case AttackMode.DirectHit:
                            lookAtPlayer = true;
                        break;
                    }

                    foreach(Flank flanks in tankFlanks)
                        flanks.Shoot();
                break;

                case EnemyState.Stop:
                    switch(currentAttack)
                    {
                        case AttackMode.Spinfire:
                            lookAtPlayer = false;

                            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                        break;

                        case AttackMode.DirectHit:
                            lookAtPlayer = true;
                        break;
                    }

                    foreach(Flank flanks in tankFlanks)
                        flanks.Shoot();
                break;

                case EnemyState.Retreat:
                    if (!createdPosition)
                    {
                        roamingPosition = new Vector3(Random.Range(minRoamingArea.x, maxRoamingArea.x), Random.Range(minRoamingArea.y, maxRoamingArea.y));
                        createdPosition = true;
                    }

                    if (this.transform.position != roamingPosition && createdPosition)
                        transform.position = Vector2.MoveTowards(transform.position, roamingPosition, moveSpeed * Time.deltaTime);
                    if (this.transform.position == roamingPosition)
                    {
                        createdPosition = false;
                        currentHealth += currentHealth / 2f;
                        retreating = false;
                    }

                    lookAtPlayer = false;
                break;
            }

            if (Vector3.Distance(transform.position, player.transform.position) < recognizeDistance)
            {
                currentState = EnemyState.Chase;

                if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
                {
                    currentState = EnemyState.Shoot;

                    if (Vector3.Distance(transform.position, player.transform.position) < stopDistance)
                    {
                        currentState = EnemyState.Stop;
                    }
                }

                else
                    currentState = EnemyState.Chase;
            }

            else
                currentState = EnemyState.Roam;
        }

        if (currentHealth <= 0)
            dead = true;

        if (!isTurret)       
            if (currentHealth <= health / 7)
                retreating = true;
    }

    bool CheckForDamageEffect()
    {
        if (!electrify && !stun)
            return true;

        return false;
    }

    void SetTankColor()
    {
        tankSkin.color = tankColor;
        outline.color = Color.Lerp(tankColor, Color.black, .25f);
    }

    void SetTankFlanks()
    {
        tankFlanks.Clear();

        foreach (Transform child in flankOrigin)
        {
            tankFlanks.Add(child.GetComponent<Flank>());

            if (this.transform.CompareTag("Enemy"))
                child.GetComponent<Flank>().currentPorter = Flank.FlankPorter.Enemy;
        }
    }

    void TurretRest()
    {
        if (currentTimeToRest <= 0)
        {
            currentShieldHealth = shieldHealth;
            currentTimeToRest = timeToRest;
            generatedSmoke = false;
            Destroy(smoke);
            resting = false;
        }

        else
        {
            if (!generatedSmoke)
            {
                smoke = Instantiate(restingEffect.gameObject, transform);
                smoke.transform.localPosition = new Vector2(0f, 0f);
                generatedSmoke = true;
            }

            currentTimeToRest -= Time.deltaTime;
        }
    }

    public void MakeDamage(float damage)
    {   
        if (isTurret)
        {   
            if (!resting)
            {
                if (currentShieldDown < 0)
                {
                    if (currentHealth <= 0)
                        dead = true;
                    else
                        currentHealth -= damage;
                }

                else
                {
                    if (currentShieldHealth <= 0)
                    {
                        currentShieldDown--;

                        if (currentShieldDown > -1)
                            resting = true;
                        else
                        {
                            GameObject tmp = Instantiate(smokeExplosionEffect.gameObject, transform);
                        }
                    }

                    else
                        currentShieldHealth -= damage;
                }
            }
        }

        else
        {
            if (currentHealth > 1)
                currentHealth -= damage;

            else
                dead = true;
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth < health)
            currentHealth += amount;
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

                if (!isTurret)   
                    tempEffect.startColor = tankColor;


                generatedEffect = true;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (Collider2D near in colliders)
            {
                if (near.GetComponent<Tank>() != null)
                {
                    near.GetComponent<Tank>().MakeDamage(damageExplosion);
                    near.GetComponent<Rigidbody2D>().AddForce(-transform.right * explosionForce);
                }
            }

            if (instEffect.Count > 0 || instEffect != null)
                foreach (GameObject inst in instEffect)
                    inst.transform.parent = null;

            if (fire)
            {
                for (int i = 0; i < instEffect.Count; i++)
                {
                    if (instEffect[i].name == "FireEffect(Clone)")
                    {
                        GameObject tmpF = instEffect[i];
                        instEffect.Remove(tmpF);
                        StartCoroutine(DestroyEffectAferTime(tmpF));
                        break;
                    }
                }
            }

            if (electrify || stun)
            {
                for (int i = 0; i < instEffect.Count; i++)
                {
                    if (instEffect[i].name == "ElectricEffect(Clone)")
                    {
                        GameObject tmpE = instEffect[i];
                        instEffect.Remove(tmpE);
                        StartCoroutine(DestroyEffectAferTime(tmpE));
                        break;
                    }
                }
            }

            switch (currentRegion)
            {
                case EnemyFaction.LightHeavy:
                    gameManager.SetLimit("LightHeavy");
                break;

                case EnemyFaction.Shield:
                    gameManager.SetLimit("Shield");
                break;

                case EnemyFaction.Taser:
                    gameManager.SetLimit("Taser");
                break;

                case EnemyFaction.Medic:
                    gameManager.SetLimit("Medic");
                break;

                case EnemyFaction.BulldozerLight:
                    gameManager.SetLimit("BulldozerLight");
                break;

                case EnemyFaction.BulldozerMedium:
                    gameManager.SetLimit("BulldozerMedium");
                break;

                case EnemyFaction.BulldozerHeavy:
                    gameManager.SetLimit("BulldozerHeavy");
                break;
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
                if (instEffect[i].name == "FireEffect(Clone)")
                {
                    GameObject tmpF = instEffect[i];
                    instEffect.Remove(tmpF);
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
        {
            fire = true;

            if (!instEffect.Contains(effect))
                FireEffect(effect);
        }

        else
            currentFireDamageDuration = fireDamageDuration;
    }

    public void CheckForElectricity(GameObject effect)
    {
        if (!electrify && !stun)
        {
            if(CheckForRandomElectric())
            {
                SetElectricEffect();

                if (!instEffect.Contains(effect))
                    ElectricEffect(effect);
            }
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
        if (currentElectricEffectDuration <= 0)
        {
            electrify = false;
            lookAtPlayer = true;
            currentElectricEffectDuration = electricEffectDuration;

            for (int i = 0; i < instEffect.Count; i++)
            {
                if (instEffect[i].name == "ElectricEffect(Clone)")
                {
                    GameObject tmpE = instEffect[i];
                    instEffect.Remove(tmpE);
                    StartCoroutine(DestroyEffectAferTime(tmpE));
                    break;
                }
            }
        }

        else
        {
            lookAtPlayer = false;

            if (timeBtwElectricEffect <= 0)
            {
                var thisRotation = transform.eulerAngles;
                thisRotation.z = Random.Range(0f, 360f);
                transform.eulerAngles = thisRotation;
                timeBtwElectricEffect = 0.5f;
            }

            else
                timeBtwElectricEffect -= Time.deltaTime;
            
            currentElectricEffectDuration -= Time.deltaTime;
        }
    }

    void Stun()
    {
        if (currentElectricEffectDuration <= 0)
        {
            stun = false;
            lookAtPlayer = true;
            currentElectricEffectDuration = electricEffectDuration;

            foreach(Flank flanks in tankFlanks)
                flanks.burstElectric = false;

            for (int i = 0; i < instEffect.Count; i++)
            {
                if (instEffect[i].name == "ElectricEffect(Clone)")
                {
                    GameObject tmpE = instEffect[i];
                    instEffect.Remove(tmpE);
                    StartCoroutine(DestroyEffectAferTime(tmpE));
                    break;
                }
            }
        }

        else
        {
            lookAtPlayer = false;

            if (timeBtwElectricEffect <= 0)
            {
                foreach(Flank flanks in tankFlanks)
                    flanks.burstElectric = true;
                
                timeBtwElectricEffect = 0.5f;
            }

            else
                timeBtwElectricEffect -= Time.deltaTime;

            currentElectricEffectDuration -= Time.deltaTime;
        }
    }

    void SetElectricEffect()
    {
        if (Random.value > 0.5)
            electrify = true;
        else
            stun = true;
    }

    void MedicBehaviour()
    {
        if (currentTimeBtwHeals <= 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, healRadius);

            foreach (Collider2D nearTanks in colliders)
            {
                if (nearTanks.GetComponent<EnemyTank>() != null)
                {
                    if (nearTanks.gameObject != this.gameObject)
                    {
                        GameObject tmpEffect = Instantiate(healEffect, transform.position, transform.rotation);
                        nearTanks.GetComponent<EnemyTank>().Heal(healAmount);
                        currentTimeBtwHeals = timeBtwHeals;
                    }
                }
            }
        }

        else
            currentTimeBtwHeals -= Time.deltaTime;
    }

    void OnDrawGizmos()
    {   
        if (canHeal)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, healRadius);
        }

        if (canExplode)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
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
