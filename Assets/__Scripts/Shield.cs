using UnityEngine;
using PlayerControls; // Reference the Hero class namespace

/// <summary>
/// Visual controller for the player's shield. 
/// Animates rotation and shifts texture offset based on current shield level.
/// </summary>
public class Shield : MonoBehaviour
{
    [Header("Shield Settings")]
    public float rotationPerSecond = 0.1f;

    [Header("Dynamic State")]
    public int levelShown = 0;
    
    private Material mat;

    void Start()
    {
        // Cache the material to modify texture offset efficiently
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Safety check to ensure the Hero instance exists
        if (Hero.S == null) return;

        // Sync visual shield level with Hero's current shield level
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        
        if (levelShown != currLevel)
        {
            levelShown = currLevel;
            // Shift texture horizontally to show the correct shield state
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }

        // Apply continuous rotation for a dynamic visual effect
        float rZ = -(rotationPerSecond * Time.time * 360f) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}


