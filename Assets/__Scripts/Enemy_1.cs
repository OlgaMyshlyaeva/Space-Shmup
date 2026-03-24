using UnityEngine;

/// <summary>
/// Specialized enemy that follows a sinusoidal wave pattern while moving down.
/// Combines custom horizontal logic with base vertical movement.
/// </summary>
public class Enemy_1 : Enemy {
    [Header("Enemy_1 Settings")]
    public float waveFrequency = 2f; // Seconds for a full sine wave cycle
    public float waveWidth = 4f;     // Horizontal amplitude
    public float waveRotY = 45f;     // Maximum banking angle during turns

    private float x0;                // Fixed horizontal center line
    private float birthTime;

    void Start() {
        x0 = pos.x;
        birthTime = Time.time;
    }

    /// <summary>
    /// Overrides Move to add horizontal oscillation and banking rotation.
    /// </summary>
    public override void Move() {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        
        // Calculate horizontal offset
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // Visual banking effect based on movement direction
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        // Inherit standard downward movement from the base class
        base.Move();
    }
}

