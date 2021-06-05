using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Tank : MonoBehaviour
{
    //Visible
    public float health;
    public float moveSpeed;
    public float fireDamageDuration;
    public float damagePerSecond;
    public float electricEffectDuration;
    public float grazeTime;
    public int maxEnemiesTargetedBy;
    public Color tankColor;
    public SpriteRenderer tankSkin;
    public SpriteRenderer outline;
    public Transform flankOrigin;
    public List<Flank> tankFlanks = new List<Flank>();
    [Header ("UI")]
    public Transform sliderPosition;
    public Slider healthSlider;


    //Invisible
    public bool fire;
    public bool canMove;
    float currentGrazeTime;
    bool electrify;
    bool stun;
    bool grazed;
    float currentFireDamageDuration;
    float currentFireLifeTime;
    float currentElectricEffectDuration;
    List<float> flanksRecoil = new List<float>();
    List<GameObject> instEffect = new List<GameObject>();
    public float currentHealth;
    float timeBtwElectricEffect;
    Vector2 mv;
    Vector2 mp;
    Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = health;
        currentFireLifeTime = 1;
        currentFireDamageDuration = fireDamageDuration;
        currentElectricEffectDuration = electricEffectDuration;
        healthSlider.maxValue = health;
        currentGrazeTime = grazeTime;

        SetTankColor();
        SetTankFlanks();
    }

    void Update()
    {   
        if (canMove)
        {
            mv.x = Input.GetAxisRaw("Horizontal");
            mv.y = Input.GetAxisRaw("Vertical");

            mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (grazed)
            {
                if (currentGrazeTime <= 0f)
                {
                    currentGrazeTime = grazeTime;
                    grazed = false;
                }

                else
                    currentGrazeTime -= Time.deltaTime;
            }
        }

        else
            mv.x = mv.y = 0;

        if (fire)
            SetOnFire();
            
        if (electrify && !stun)
            Electrified();

        if (!electrify && stun)
            Stun();
        
        UI();
    }

    void FixedUpdate()
    {   
        if (canMove)
        {
            if (Input.GetKey(KeyCode.Mouse0))
                foreach (Flank flanks in tankFlanks)
                    flanks.Shoot();

            rb.MovePosition(rb.position + mv.normalized * moveSpeed * Time.fixedDeltaTime);

            Vector2 lookDir = mp - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }

    void SetTankColor()
    {
        tankSkin.color = tankColor;
        outline.color = Color.Lerp(tankColor, Color.black, .25f);
    }

    void SetTankFlanks()
    {
        foreach (Transform child in flankOrigin)
        {
            tankFlanks.Add(child.GetComponent<Flank>());
            flanksRecoil.Add(child.GetComponent<Flank>().recoil);
        }
    }

    public void MakeDamage(float damage)
    {   
        if (!grazed)
        {
            if (currentHealth <= 0)
                Debug.Log("No more UGH *sad face*");
            else
                currentHealth -= damage;
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth < health)
            currentHealth += amount;
    }

    void UI()
    {
        healthSlider.value = currentHealth;
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
    }

    bool CheckForRandomElectric()
    {
        return (Random.value > 0.9);
    }

    void Electrified()
    {
        if (currentElectricEffectDuration <= 0)
        {
            electrify = false;
            canMove = true;
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
            canMove = false;

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
            canMove = true;
            currentElectricEffectDuration = electricEffectDuration;

            foreach(Flank flanks in tankFlanks)
                flanks.burstElectric = false;
            
            for (int i = 0; i < tankFlanks.Count; i++)
            {
                tankFlanks[i].burstElectric = false;
                tankFlanks[i].recoil = flanksRecoil[i];
            }

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
            foreach(Flank flanks in tankFlanks)
                flanks.recoil = 0f;

            canMove = false;

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

    IEnumerator DestroyEffectAferTime(GameObject effectTo)
    {
        effectTo.GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1f);
        Destroy(effectTo);
    }
}
