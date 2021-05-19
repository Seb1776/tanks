using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flank : MonoBehaviour
{
    //Visible
    public enum FlankPorter {Tank, Enemy}
    public FlankPorter currentPorter;
    public enum FlankType {Normal, Fire, Explosive, Sniper, Electric, Piercing, Smoker}
    public FlankType currentFlank;
    public enum FireMode {Single, Burst, Auto, Electric}
    public FireMode currentFireMode;
    public Transform firePoint;
    public float reloadTime;
    public float recoil;
    [Header("Burst Fire Mode Properties")]
    public float bulletsPerReload;
    [Header ("Auto Fire Magazine")]
    public int bulletsInMag;
    [Header ("Random Burst Electric Effect")]
    public Vector2 burstBulletElectric;
    [HideInInspector]
    public bool burstElectric;


    //Invisible
    GameObject tank;
    int burstCycle;
    int currentFiredShots;
    float originalRotation;
    float randomBulletAmount;
    float currentRandomBulletAmount;
    bool canShoot = true;
    bool burst;
    bool createdAmount;
    Bullet bullet;
    float currentReloadTime;
    float currentBurstBulletPerSecond;
    float autoTimeBtwShots;


    void Start()
    {
        SelectBullet();

        tank = transform.parent.transform.parent.transform.parent.transform.parent.gameObject;
        currentReloadTime = reloadTime;
        originalRotation = firePoint.transform.eulerAngles.z;
        currentFiredShots = bulletsInMag;
    }

    void Update()
    {
        if (!canShoot)
            Reload();
        
        if (burst)
            BurstShoot();
        
        if (burstElectric)
            ShootStun();
    }

    public void Shoot()
    {
        if (canShoot)
        {
            GameObject bulletPref = null;

            switch(currentFireMode)
            {
                case FireMode.Single:
                    bulletPref = Instantiate(bullet.gameObject, firePoint.position, firePoint.rotation);
                    AccomodateBullet(bulletPref);
                    ApplyRecoil();
                    canShoot = false;
                break;

                case FireMode.Burst:
                    burst = true;
                break;

                case FireMode.Auto:
                    if (currentFiredShots > 0)
                    {
                        if (autoTimeBtwShots <= 0)
                        {
                            bulletPref = Instantiate(bullet.gameObject, firePoint.position, firePoint.rotation);
                            AccomodateBullet(bulletPref);
                            ApplyRecoil();
                            currentFiredShots--;
                            autoTimeBtwShots = 0.1f;
                        }

                        else
                            autoTimeBtwShots -= Time.deltaTime;
                    }

                    else
                        canShoot = false;
                break;
            }
        }
    }

    void AccomodateBullet(GameObject bullet)
    {
        switch (currentPorter)
        {
            case FlankPorter.Tank:
                bullet.gameObject.tag = "Bullet";
            break;

            case FlankPorter.Enemy:
                bullet.gameObject.tag = "EnemyBullet";
            break;
        }
    }

    void ApplyRecoil()
    {
        tank.GetComponent<Rigidbody2D>().AddForce(-transform.right * recoil);
    }

    void BurstShoot()
    {
        for (int i = 0; i < bulletsPerReload; i++)
        {
            if (currentBurstBulletPerSecond <= 0)
            {
                GameObject bulletPref = Instantiate(bullet.gameObject, firePoint.position, firePoint.rotation);
                AccomodateBullet(bulletPref);
                ApplyRecoil();
                currentBurstBulletPerSecond = 0.4f;
                burstCycle++;
            }

            else
            {
                currentBurstBulletPerSecond -= Time.deltaTime;
            }
        }

        if (burstCycle >= bulletsPerReload)
        {
            canShoot = false;
            burst = false;
            burstCycle = 0;
        }
    }

    public void ShootStun()
    {
        if (!createdAmount)
        {
            randomBulletAmount = Random.Range(burstBulletElectric.x, burstBulletElectric.y);
            currentRandomBulletAmount = randomBulletAmount;
            burstCycle = (int)randomBulletAmount;
            createdAmount = true;
        }

        for (int i = 0; i < randomBulletAmount; i++)
        {
            if (currentBurstBulletPerSecond <= 0)
            {
                GameObject bulletPref = Instantiate(bullet.gameObject, firePoint.position, firePoint.rotation);
                AccomodateBullet(bulletPref);
                ApplyRecoil();
                currentBurstBulletPerSecond = 1.5f;
                burstCycle++;
            }

            else
                currentBurstBulletPerSecond -= Time.deltaTime;
        }

        if (burstCycle >= randomBulletAmount)
        {
            burstCycle = 0;
            createdAmount = false;
        }
    }

    void Reload()
    {
        burst = false;

        if (currentReloadTime <= 0)
        {
            canShoot = true;
            currentReloadTime = reloadTime;

            if (currentFireMode == FireMode.Auto)
                currentFiredShots = bulletsInMag;
        }

        else
            currentReloadTime -= Time.deltaTime;
    }

    void SelectBullet()
    {
        switch (currentFlank)
        {
            case FlankType.Normal:
                bullet = Resources.Load<Bullet>("Prefabs/Bullets/RegularBullet");
            break;

            case FlankType.Fire:
                bullet = Resources.Load<Bullet>("Prefabs/Bullets/FireBullet");
            break;

            case FlankType.Explosive:
                bullet = Resources.Load<Bullet>("Prefabs/Bullets/ExplosiveBullet");
            break;

            case FlankType.Sniper:
                bullet = Resources.Load<Bullet>("Prefabs/Bullets/SniperBullet");
            break;

            case FlankType.Electric:
                bullet = Resources.Load<Bullet>("Prefabs/Bullets/ElectricBullet");
            break;

            case FlankType.Piercing:
                bullet = Resources.Load<Bullet>("Prefabs/Bullets/PiercingBullet");
            break;

            case FlankType.Smoker:
                bullet = Resources.Load<Bullet>("Prefabs/Bullets/SmokerBullet");
            break;

            default:
                Debug.LogError("The bullet type couldn't be found; Current flank type: " + currentFlank);
            break;
        }
    }
}
