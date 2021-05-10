using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flank : MonoBehaviour
{
    //Visible
    public enum FlankType {Normal, Fire, Explosive, Sniper, Electric, Piercing}
    public FlankType currentFlank;
    public enum FireMode {Single, Burst, Auto}
    public FireMode currentFireMode;
    public Transform firePoint;
    public float reloadTime;
    public float recoil;
    [Header("Burst Fire Mode Properties")]
    public float bulletsPerReload;


    //Invisible
    GameObject tank;
    int burstCycle;
    float originalRotation;
    bool canShoot = true;
    bool burst;
    Bullet bullet;
    float currentReloadTime;
    float currentBurstBulletPerSecond;


    void Start()
    {
        SelectBullet();

        tank = transform.parent.transform.parent.transform.parent.transform.parent.gameObject;
        currentReloadTime = reloadTime;
        originalRotation = firePoint.transform.eulerAngles.z;
    }

    void Update()
    {
        if (!canShoot)
            Reload();
        
        if (burst)
            BurstShoot();
    }

    public void Shoot()
    {
        if (canShoot)
        {
            switch(currentFireMode)
            {
                case FireMode.Single:
                    GameObject bulletPref = Instantiate(bullet.gameObject, firePoint.position, firePoint.rotation);
                    ApplyRecoil();
                    canShoot = false;
                break;

                case FireMode.Burst:
                    burst = true;
                break;
            }
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

    void Reload()
    {
        burst = false;

        if (currentReloadTime <= 0)
        {
            canShoot = true;
            currentReloadTime = reloadTime;
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

            default:
                Debug.LogError("The bullet type couldn't be found; Current flank type: " + currentFlank);
            break;
        }
    }
}
