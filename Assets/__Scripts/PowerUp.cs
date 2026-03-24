using UnityEngine;
using TMPro;

/// <summary>
/// Handles the behavior of PowerUp items, including movement, 
/// rotation, and smooth fading before destruction.
/// </summary>
public class PowerUp : MonoBehaviour
{
    [Header("Movement & Rotation")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(5f, 7f);
    public float lifeTime = 6f; // Total existence time in seconds
    public float fadeTime = 4f; // Time spent fading out before destruction

    [Header("Dynamic State")]
    public WeaponType type;
    public GameObject cube; 
    public TextMeshPro letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    void Awake()
    {
        // Cache references to components and child objects
        cube = transform.Find("Cube").gameObject;
        cubeRend = cube.GetComponent<Renderer>();
        letter = GetComponentInChildren<TextMeshPro>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();

        // Set random downward drift speed
        float descentSpeed = Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.linearVelocity = new Vector3(0, -descentSpeed, 0);

        // Generate random rotation speed for each axis
        rotPerSecond = new Vector3(
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }

    void Update()
    {
        // Rotate the inner cube independently of the parent object
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // Calculate normalized time for the fade-out effect (0 to 1)
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;

        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // Apply fading effect to materials and text
        if (u > 0)
        {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;

            // Fade out the letter slightly slower than the cube
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        // Clean up if the PowerUp moves off screen
        if (bndCheck != null && !bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Configures the PowerUp appearance based on its weapon type.
    /// </summary>
    public void SetType(WeaponType wt)
    {
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        cubeRend.material.color = def.color;
        letter.text = def.letter;
        type = wt;
    }

    /// <summary>
    /// Called when the player ship collects this PowerUp.
    /// </summary>
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}
