using UnityEngine;

/// <summary>
/// Base class for all enemy types. 
/// Handles core mechanics: movement, health management, and visual damage feedback.
/// Demonstrates OOP principles through virtual methods and inheritance.
/// </summary>
public class Enemy : MonoBehaviour {
    [Header("Inscribed: Enemy Settings")]
    public float speed = 10f;             // Forward movement speed
    public float health = 1;              // Total hit points
    public float powerUpDropChance = 0.1f;
    public int score = 100;               // Points awarded upon destruction
    public float showDamageDuration = 0.1f; // Duration of the red flash effect

    [Header("Dynamic State")]
    protected Color[] originalColors;     // Stores original material colors
    protected Material[] materials;       // References to all renderers' materials
    public bool notifiedOfDestruction = false; // Prevents multiple destruction calls
    protected BoundsCheck bndCheck;       // Component to keep the enemy on screen

    protected virtual void Awake() {
        bndCheck = GetComponent<BoundsCheck>();
        notifiedOfDestruction=false;
        // Cache all materials to perform damage-flash effects efficiently
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++) {
            originalColors[i] = materials[i].color;
        }
    }

    public Vector3 pos {
        get { return transform.position; }
        set { transform.position = value; }
    }

    void Update() {
        Move();
        // Auto-destruction logic when the enemy leaves the bottom boundary
        if (bndCheck != null && bndCheck.offDown) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Basic linear downward movement. Can be overridden for custom flight paths.
    /// </summary>
    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    /// <summary>
    /// Processes incoming damage and triggers death if health reaches zero.
    /// </summary>
    public virtual void TakeDamage(GameObject goHit, WeaponType weaponType) {
        health -= Main.GetWeaponDefinition(weaponType).damageOnHit;
        
        if (health <= 0 && !notifiedOfDestruction) {
            Main.S.ShipDestroyed(this);
            notifiedOfDestruction = true;
            Destroy(gameObject);
        } else {
            ShowDamage(); // Visual feedback for non-lethal hits
        }
    }

    /// <summary>
    /// Visual effect: briefly changes all materials to red when damaged.
    /// </summary>
    void ShowDamage() {
        foreach (Material m in materials) {
            m.color = Color.red;
        }
        Invoke(nameof(UnShowDamage), showDamageDuration);
    }

    void UnShowDamage() {
        for (int i = 0; i < materials.Length; i++) {
            materials[i].color = originalColors[i];
        }
    }

    /// <summary>
    /// Collision handler for projectiles. Validates if the enemy is on-screen before taking damage.
    /// </summary>
    public virtual void OnCollisionEnter(Collision coll) {
        GameObject otherGO = coll.gameObject;
        if (otherGO.CompareTag("ProjectileHero")) {
            // Ignore hits if the enemy hasn't fully entered the play area
            /* if (bndCheck != null && !bndCheck.isOnScreen) {
                Destroy(otherGO);
                return;
            }
            */
            Projectile p = otherGO.GetComponent<Projectile>();
            TakeDamage(gameObject, p.type);
            Destroy(otherGO);
        }
    }
}
