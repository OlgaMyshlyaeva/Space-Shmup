using UnityEngine;

/// <summary>
/// Adjusts the Orthographic Camera size to fit a specific width in Unity units.
/// This ensures the game looks consistent across different aspect ratios (Portrait/Landscape).
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    [Header("Adjust Settings")]
    [Tooltip("The desired width of the game area in Unity units.")]
    public float targetWidth = 10f;

    private Camera _mainCamera;

    void Awake()
    {
        _mainCamera = GetComponent<Camera>();
        ApplyCameraSize();
    }

    /// <summary>
    /// Calculates and sets the orthographic size based on the target width and current screen aspect ratio.
    /// Formula: Size = (Target Width / Aspect Ratio) / 2
    /// </summary>
    public void ApplyCameraSize()
    {
        if (_mainCamera == null) return;

        // Get the current screen aspect ratio (Width / Height)
        float currentAspectRatio = (float)Screen.width / Screen.height;

        // Calculate the required orthographic size to maintain targetWidth horizontally
        float requiredSize = (targetWidth / currentAspectRatio) * 0.5f;

        // Apply the new size to the camera
        _mainCamera.orthographicSize = requiredSize;
    }

    // This allows you to see the result instantly in the Unity Editor without pressing Play
#if UNITY_EDITOR
    void OnValidate()
    {
        if (_mainCamera == null) _mainCamera = GetComponent<Camera>();
        ApplyCameraSize();
    }

    void Update()
    {
        // Helpful for testing by resizing the Game window in Editor
        if (!Application.isPlaying) ApplyCameraSize();
    }
#endif
}
