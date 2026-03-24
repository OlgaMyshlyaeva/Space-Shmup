using UnityEngine;

/// <summary>
/// Data container for projectiles. Synchronizes physics and visual color with weapon type.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Inscribed")]
    public Rigidbody rigid; // Reference for the physics engine used by Weapon.cs
    private Renderer rend;
    private BoundsCheck bndCheck;

    [SerializeField] private WeaponType _type;

    public WeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        bndCheck = GetComponent<BoundsCheck>();
    }

    void Update()
    {
        if (bndCheck != null && bndCheck.offUp)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates the projectile's color to match its weapon definition.
    /// </summary>
    public void SetType(WeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        if (rend != null) rend.material.color = def.projectileColor;
    }
}

