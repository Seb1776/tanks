using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTank : MonoBehaviour
{
    //Visible
    [Header ("Debug")]
    public bool showDebugGizmos;
    [SerializeField]
    bool obstacleInBetween;
    public enum EnemyFaction {LightHeavy, Shield, Sniper, Taser, Medic, Smoker, BulldozerLight, BulldozerMedium, BulldozerHeavy, Turret}
    public EnemyFaction currentRegion;
    public enum EnemyState {Roam, Chase, Shoot, Stop, Retreat}
    public EnemyState currentState;
    public enum AttackMode {Spinfire, DirectHit}
    public AttackMode currentAttack;
    public float health;
    public int score;
    public float moveSpeed;
    public float attackDistance;
    public float recognizeDistance;
    public float stopDistance;
    public float rotationSpeed;
    public float lookSpeed;
    public float moveSpace;
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
    Vector3[] path;
    int targetIndex;
    public bool fire;
    [SerializeField]
    Vector2 minRoamingArea;
    [SerializeField]
    Vector2 maxRoamingArea;
    public float currentHealth;
    public float setDamage;
    Vector2 areaSum = Vector2.zero;
    bool stun;
    bool electrify;
    bool lookAtPlayer = true;
    float timeToGetThere;
    List<GameObject> instEffect = new List<GameObject>();
    public List<Flank> tankFlanks = new List<Flank>();
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
    float nearCount;
    int currentShieldDown;
    bool dead;
    bool generatedEffect;
    bool generatedSmoke;
    bool createdPosition;
    bool createdDistance;
    bool retreating;
    GameObject smoke;
    GameObject player;
    GameManager gameManager;
    Vector3 roamingPosition;
    NavMeshAgent agent;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        SetTankColor();
        SetTankFlanks();
        AssignValuesToNavMesh();
    }

    void Start()
    {
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
            
            agent.SetDestination(player.transform.position);

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

    public void OnDrawGizmos() 
    {   
        if (showDebugGizmos)
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

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, moveSpace);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);
            player = GameObject.FindGameObjectWithTag("Player");
            Gizmos.color = Color.black;

            if (player != null)
            {
                RaycastHit2D hit2D = Physics2D.Linecast(transform.position, player.transform.position);

                if (Vector2.Distance(transform.position, player.transform.position) < attackDistance)
                {
                    if (hit2D.transform.CompareTag("Player"))
                        Debug.DrawLine(transform.position, player.transform.position);

                    else
                        Debug.DrawLine(transform.position, hit2D.point);
                }
            }
        }
	}

    void AssignValuesToNavMesh()
    {
        agent.speed = moveSpeed;
        agent.stoppingDistance = stopDistance;
        agent.radius += .5f;
    }

    void EnemyMovement(Vector3 target)
    {
        //transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, moveSpace);

        foreach (Collider2D near in colliders)
        {
            if (near.GetComponent<EnemyTank>() != null && near.transform != transform)
            {
                Vector2 difference = transform.position - near.transform.position;
                difference = difference.normalized / Mathf.Abs(difference.magnitude);
                areaSum += difference;
                nearCount++;
            }
        }

        if (nearCount > 0)
        {
            areaSum /= nearCount;
            areaSum = areaSum.normalized * (moveSpeed);
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)areaSum, (moveSpeed / 2f) * Time.deltaTime);
        }

        RaycastHit2D hit2D = Physics2D.Linecast(transform.position, player.transform.position);

        if (hit2D.transform.CompareTag("Player") || hit2D.transform.CompareTag("Bullet") || hit2D.transform.CompareTag("EnemyBullet") || hit2D.transform.CompareTag("Shield"))
        {
            LookAtTarget(player.transform.position);
            obstacleInBetween = false;
        }

        else
        {
            LookAtTarget(hit2D.point);
            obstacleInBetween = true;
        }
        
    }

    void LookAtTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.Normalize();
        float zAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion lookRot = Quaternion.Euler(0f, 0f, zAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, lookSpeed * Time.deltaTime);
    }

    void TankAI()
    {
        if (lookAtPlayer)
            LookAtTarget(player.transform.position);

        if (CheckForDamageEffect())
        {
            if (retreating)
                currentState = EnemyState.Retreat;
            
            if (canHeal)
                MedicBehaviour();

            switch (currentState)
            {
                /*case EnemyState.Roam:
                    if (!createdPosition)
                    {
                        roamingPosition = new Vector3(Random.Range(minRoamingArea.x, maxRoamingArea.x), Random.Range(minRoamingArea.y, maxRoamingArea.y));
                        createdPosition = true;
                    }

                    if (this.transform.position != roamingPosition && createdPosition)
                        EnemyMovement(roamingPosition);

                    if (!createdDistance)
                    {
                        float distance = Vector3.Distance(transform.position, roamingPosition);
                        timeToGetThere = distance / moveSpeed;
                        createdDistance = true;
                    }

                    else
                    {
                        if (timeToGetThere <= 0 || transform.position == roamingPosition)
                        {
                            createdDistance = false;
                            createdPosition = false;
                        }

                        else
                            timeToGetThere -= Time.deltaTime;
                    }
                    
                    lookAtPlayer = false;
                break;*/

                case EnemyState.Chase:
                    EnemyMovement(player.transform.position);
                break;

                case EnemyState.Shoot:
                    EnemyMovement(player.transform.position);

                    if (!obstacleInBetween)
                    { 
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
                    }
                break;

                case EnemyState.Stop:
                    /*switch(currentAttack)
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
                        flanks.Shoot();*/
                break;

                case EnemyState.Retreat:
                    /*if (!createdPosition)
                    {
                        roamingPosition = new Vector3(Random.Range(minRoamingArea.x, maxRoamingArea.x), Random.Range(minRoamingArea.y, maxRoamingArea.y));
                        createdPosition = true;
                    }

                    if (this.transform.position != roamingPosition && createdPosition)
                        EnemyMovement(roamingPosition);

                    if (!createdDistance)
                    {
                        float distance = Vector3.Distance(transform.position, roamingPosition);
                        timeToGetThere = distance / moveSpeed;
                        createdDistance = true;
                    }

                    else
                    {
                        if (timeToGetThere <= 0 || transform.position == roamingPosition)
                        {
                            createdDistance = false;
                            createdPosition = false;
                            currentHealth += currentHealth / 6f;
                            retreating = false;
                        }

                        else
                            timeToGetThere -= Time.deltaTime;
                    }

                    lookAtPlayer = false;*/
                break;
            }

            if (Vector3.Distance(transform.position, player.transform.position) < recognizeDistance)
            {
                currentState = EnemyState.Chase;

                if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
                    currentState = EnemyState.Shoot;

                else
                    currentState = EnemyState.Chase;
            }
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
                            GameObject tmp = Instantiate(smokeExplosionEffect.gameObject, transform.position, transform.rotation);
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

            foreach (GameObject gameObject in instEffect)
                Destroy(gameObject);

            switch (currentRegion)
            {
                case EnemyFaction.LightHeavy:
                    gameManager.SetLimit("LightHeavy");
                break;

                case EnemyFaction.Shield:
                    gameManager.SetLimit("Shield");
                break;

                case EnemyFaction.Sniper:
                    gameManager.SetLimit("Sniper");
                break;

                case EnemyFaction.Taser:
                    gameManager.SetLimit("Taser");
                break;

                case EnemyFaction.Medic:
                    gameManager.SetLimit("Medic");
                break;

                case EnemyFaction.Smoker:
                    gameManager.SetLimit("Smoker");
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

            gameManager.score += score;

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
        return (Random.value > 0.7);
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
