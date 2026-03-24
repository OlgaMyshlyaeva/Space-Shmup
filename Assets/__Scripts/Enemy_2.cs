using UnityEngine;

/// <summary>
/// Specialized enemy that traverses the screen horizontally between two random points.
/// Uses a custom easing function (S-Curve) for non-linear movement.
/// </summary>
public class Enemy_2 : Enemy {
    [Header("Enemy_2 Settings")]
    public float sinEccentricity = 0.6f; // Controls the "curve" intensity
    public float lifeTime = 10f;         // Time to cross the screen

    public Vector3 p0, p1;               // Start and End interpolation points
    public float birthTime;

    void Start() {
        // Define screen-space boundaries for spawning
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // Randomly flip direction (left-to-right or right-to-left)
        if (Random.value > 0.5f) {
            p0.x *= -1;
            p1.x *= -1;
        }
        birthTime = Time.time;
    }

    /// <summary>
    /// Overrides Move with a purely interpolation-based system.
    /// Does NOT call base.Move() as it manages all axes independently.
    /// </summary>
    public override void Move() {
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1) {
            Destroy(this.gameObject);
            return;
        }

        // Apply sine-based easing for smooth acceleration/deceleration
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));
        pos = (1 - u) * p0 + u * p1;
    }
}
