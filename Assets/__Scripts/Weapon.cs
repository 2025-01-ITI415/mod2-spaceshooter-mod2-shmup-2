using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is an enum of the various possible weapon types.
/// It also includes a "shield" type to allow a shield PowerUp.
/// Items marked [NI] below are Not Implemented in this book.
/// </summary>
public enum eWeaponType
{
    none,       // The default / no weapon
    blaster,    // A simple blaster
    spread,     // Multiple shots simultaneously
    phaser,     // [NI] Shots that move in waves
    missile,    // [NI] Homing missiles

    laser,      // [NI] Damage over time
    shield,      // Raise shieldLevel
    enemyWeapon
}


/// <summary>
/// The WeaponDefinition class allows you to set the properties
///   of a specific weapon in the Inspector. The Main class has
///   an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]                                                         // a
public class WeaponDefinition
{                                               // b
    public eWeaponType type = eWeaponType.none;
    [Tooltip("Letter to show on the PowerUp Cube")]                           // c
    public string letter;
    [Tooltip("Color of PowerUp Cube")]
    public Color powerUpColor = Color.white;                           // d
    [Tooltip("Prefab of Weapon model that is attached to the Player Ship")]
    public GameObject weaponModelPrefab;
    [Tooltip("Prefab of projectile that is fired")]
    public GameObject projectilePrefab;
    [Tooltip("Color of the Projectile that is fired")]
    public Color projectileColor = Color.white;                        // d
    [Tooltip("Damage caused when a single Projectile hits an Enemy")]
    public float damageOnHit = 0;
    [Tooltip("Damage caused per second by the Laser [Not Implemented]")]
    public float damagePerSec = 0;
    [Tooltip("Seconds to delay between shots")]
    public float delayBetweenShots = 0;
    [Tooltip("Velocity of individual Projectiles")]
    public float velocity = 50;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Dynamic")]                                                        // a
    [SerializeField]                                                           // a
    [Tooltip("Setting this manually while playing does not work properly.")]   // a
    private eWeaponType _type = eWeaponType.none;
    public WeaponDefinition def;
    public WeaponDefinition enemyDef;
    public float nextShotTime; // Time the Weapon will fire next

    private GameObject weaponModel;
    private Transform shotPointTrans;

    private GameObject enemyProjectile;
    private float enemyFireRate;
    private float nextEnemyFireTime;

    void Start()
    {
        // Set up PROJECTILE_ANCHOR if it has not already been done
        if (PROJECTILE_ANCHOR == null)
        {                                       // b
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        shotPointTrans = transform.GetChild(0);                              // c

        // Call SetType() for the default _type set in the Inspector
        SetType(_type);                                                      // d

        // Find the fireEvent of a Hero Component in the parent hierarchy
        Hero hero = GetComponentInParent<Hero>();                              // e
        Enemy enemy = GetComponentInParent<Enemy>();
        if (hero != null) hero.fireEvent += Fire;
        if (enemy != null) {
            enemyProjectile = enemy.projectilePrefab;
            enemyFireRate = enemy.fireRate;
            nextEnemyFireTime = Time.time + enemyFireRate;
            enemyDef = Main.GET_WEAPON_DEFINITION(eWeaponType.enemyWeapon);
            _type = eWeaponType.enemyWeapon;
        }
    }

    public eWeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    public void SetType(eWeaponType wt)
    {
        _type = wt;
        if (type == eWeaponType.none)
        {                                       // f
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        // Get the WeaponDefinition for this type from Main
        def = Main.GET_WEAPON_DEFINITION(_type);
        // Destroy any old model and then attach a model for this weapon     // g
        if (weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        nextShotTime = 0; // You can fire immediately after _type is set.    // h
    }

    private void Fire()
    {
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;                         // i
        // If it hasn’t been enough time between shots, return
        if (Time.time < nextShotTime) return;                              // j

        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;

        switch (type)
        {                                                      // k
            case eWeaponType.blaster:
                p = MakeHeroProjectile();
                p.vel = vel;
                break;

            case eWeaponType.spread:                                         // l
                p = MakeHeroProjectile();
                p.vel = vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;
            case eWeaponType.laser:
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis( 0, Vector3.back);
                p.vel = p.transform.rotation * vel;

                break;
            case eWeaponType.phaser:
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(0, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-1, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-2, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-3, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-4, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-5, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-6, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-8, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-9, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p.transform.rotation = Quaternion.AngleAxis(1, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(2, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(3, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(4, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(5, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(6, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(8, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(9, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeHeroProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;
        }
    }

    public void EnemyFire()
    {
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        // If it hasn’t been enough time between shots, return
        if (Time.time < nextShotTime) return;

        ProjectileEnemy p;
        Vector3 vel = Vector3.down * enemyDef.velocity;

        if (type == eWeaponType.enemyWeapon)
        {
            p = MakeEnemyProjectile();
            p.vel = vel;
        }
    }

    private ProjectileHero MakeHeroProjectile()
    {                                 // m
        GameObject go;
        go = Instantiate<GameObject>(def.projectilePrefab, PROJECTILE_ANCHOR); // n
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;                                                            // o
        p.transform.position = pos;

        p.type = type;
        nextShotTime = Time.time + def.delayBetweenShots;                    // p
        return (p);
    }

    private ProjectileEnemy MakeEnemyProjectile()
    {
        GameObject go;
        go = Instantiate<GameObject>(enemyDef.projectilePrefab);
        ProjectileEnemy p = go.GetComponent<ProjectileEnemy>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;
        p.transform.position = pos;

        p.type = type;
        nextShotTime = Time.time + enemyDef.delayBetweenShots;
        return (p);
    }
}


