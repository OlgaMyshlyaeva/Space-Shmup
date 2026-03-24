using UnityEngine;
using PlayerControls;

/// <summary>
/// Enum defining all available weapon types in the game.
/// </summary>
public enum WeaponType { none, blaster, spread, phaser, missile, laser, shield, life }

/// <summary>
/// Data container for weapon statistics.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;
    public Color color = Color.white;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20f;
}

/// <summary>
/// Main weapon controller. Manages shooting patterns and projectile instantiation.
/// </summary>
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Settings")]
    [SerializeField] private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar; 
    public GameObject projectilePrefab;
    
    [Header("State")]
    public float lastShotTime;
    public bool isEnemy = true;
    private Renderer collarRend;

    void Start()
    {
        // Set up the anchor for all projectiles to keep the Hierarchy clean
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // Check if this weapon belongs to the Hero
        Hero rootHero = GetComponentInParent<Hero>();
        if (rootHero != null)
        {
            rootHero.fireDelegate += Fire;
            isEnemy = false;
        }
        
        // FAILSAFE: If no type is set in Inspector, default to Blaster
        if (_type == WeaponType.none) _type = WeaponType.blaster;

        SetType(_type);
    }

    public WeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (_type == WeaponType.none)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        def = Main.GetWeaponDefinition(_type);

        if (collar != null)
        {
            collarRend = collar.GetComponent<Renderer>();
            if (collarRend != null) collarRend.material.color = def.color;
        }
        lastShotTime = 0;
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy) return;
        if (Time.time - lastShotTime < def.delayBetweenShots) return;

        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0) vel.y = -vel.y; 

        // Pattern logic based on current weapon type
        switch (type)
        {
            case WeaponType.blaster:
                Projectile p = MakeProjectile();
                p.rigid.linearVelocity = vel;
                break;

            case WeaponType.spread:
                // Triple spread shot pattern
                Projectile p1 = MakeProjectile(); p1.rigid.linearVelocity = vel;
                Projectile p2 = MakeProjectile(); 
                p2.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p2.rigid.linearVelocity = p2.transform.rotation * vel;
                Projectile p3 = MakeProjectile(); 
                p3.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p3.rigid.linearVelocity = p3.transform.rotation * vel;
                break;

            case WeaponType.phaser:
                // Dual parallel high-speed projectiles
                float offset = 0.15f;
                Projectile phL = MakeProjectile();
                phL.transform.position += transform.right * -offset;
                phL.rigid.linearVelocity = vel * 1.25f;
                Projectile phR = MakeProjectile();
                phR.transform.position += transform.right * offset;
                phR.rigid.linearVelocity = vel * 1.25f;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(projectilePrefab);
        
        // Assign layers and tags based on ownership
        if (!isEnemy)
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        
        return p;
    }
}

