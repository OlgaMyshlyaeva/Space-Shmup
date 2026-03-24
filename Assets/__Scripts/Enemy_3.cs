using UnityEngine;

/// <summary>
/// Enemy_3 moves along a 3-point Bezier curve.
/// Demonstrates advanced interpolation and easing functions for organic movement.
/// </summary>
public class Enemy_3 : Enemy
{
    [Header("Enemy_3 Settings")]
    public float lifeTime = 5f;

    [Header("Dynamic State")]
    public Vector3[] points;
    public float birthTime;

    void Start()
    {
        points = new Vector3[3]; 
        points[0] = pos;

        // Set screen boundaries for point generation
        float xMin = -bndCheck.camWidth - bndCheck.radius;
        float xMax = bndCheck.camWidth + bndCheck.radius;

        // Generate random control points for the curve
        Vector3 v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndCheck.camHeight * Random.Range(2.75f, 2f);
        points[1] = v;

        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;

        birthTime = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // Apply sine-based easing to make movement speed variable
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);

        // Standard 3-point Bezier curve calculation
        Vector3 p01, p12;
        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        
        // Final position on the curve
        pos = (1 - u) * p01 + u * p12;

        // base.Move() removed to prevent conflicting downward movement
    }
}
