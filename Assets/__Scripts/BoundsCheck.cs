using UnityEngine;

/// <summary>
/// Keeps a GameObject within the bounds of the Main Camera.
/// Works only with an Orthographic Main Camera positioned at [0,0,0].
/// </summary>
public class BoundsCheck : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Dynamic State")]
    public bool isOnScreen = true;
    public float camWidth;
    public float camHeight;

    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown;

    void Awake()
    {
        // Calculate camera bounds based on orthographic size and aspect ratio
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        isOnScreen = true;
        offRight = offLeft = offUp = offDown = false;

        // Check horizontal bounds
        if (pos.x > camWidth - radius)
        {
            pos.x = camWidth - radius;
            offRight = true;
        }
        else if (pos.x < -camWidth + radius)
        {
            pos.x = -camWidth + radius;
            offLeft = true;
        }

        // Check vertical bounds
        if (pos.y > camHeight - radius)
        {
            pos.y = camHeight - radius;
            offUp = true;
        }
        else if (pos.y < -camHeight + radius)
        {
            pos.y = -camHeight + radius;
            offDown = true;
        }

        // Determine if the object is still within view
        isOnScreen = !(offRight || offLeft || offUp || offDown);

        // If restricted, force the object back into the camera view
        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            isOnScreen = true;
            offRight = offLeft = offUp = offDown = false;
        }
    }

    void OnDrawGizmos()
    {
        // Visualize the boundary in the Scene view for debugging
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
