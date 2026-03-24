using UnityEngine;

/// <summary>
/// Creates a seamless scrolling background effect (Parallax).
/// Supports vertical scrolling and slight horizontal reaction to player movement.
/// </summary>
public class Parallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    public GameObject poi;             // Point of Interest (Player's ship)
    public GameObject[] panels;        // Background panels to loop
    public float scrollSpeed = -30f;
    public float motionMult = 0.25f;   // How much the background reacts to ship's X movement

    private float panelHt;             // Height of a single panel
    private float depth;               // Z-depth of the background panels

    void Start()
    {
        // Safety check for assigned panels
        if (panels == null || panels.Length < 2)
        {
            Debug.LogError("Parallax: At least 2 panels must be assigned in the Inspector!");
            return;
        }

        // Initialize panel height and depth from the first assigned panel
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;

        // Set initial positions for a seamless start
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHt, depth);
    }

    void Update()
    {
        // Calculate vertical offset based on time and scroll speed
        float tY = (Time.time * scrollSpeed) % panelHt + (panelHt * 0.5f);
        float tX = 0f;

        // React horizontally to the player's ship position
        if (poi != null)
        {
            tX = -poi.transform.position.x * motionMult;
        }

        // Position the primary panel
        panels[0].transform.position = new Vector3(tX, tY, depth);

        // Position the secondary panel to create a seamless loop
        if (tY >= 0)
        {
            panels[1].transform.position = new Vector3(tX, tY - panelHt, depth);
        }
        else
        {
            panels[1].transform.position = new Vector3(tX, tY + panelHt, depth);
        }
    }
}
