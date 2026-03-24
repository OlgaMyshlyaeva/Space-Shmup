using UnityEngine;

// --- ВАЖНО: Этот класс Part должен быть ВНЕ класса Enemy_4 или внутри него ---
[System.Serializable]
public class Part {
    public string name;
    public float health;
    public string[] protectedBy;
    [HideInInspector] public GameObject go;
    [HideInInspector] public Material mat;
    [HideInInspector] public Color originalColor; // Добавили, чтобы он не становился белым
}

/// <summary>
/// Boss enemy with multiple parts. 
/// Requires the 'Part' class definition above to function.
/// </summary>
public class Enemy_4 : Enemy {
    [Header("Inscribed: Enemy_4 Settings")]
    public Part[] parts;

    void Start() {
        foreach (Part prt in parts) {
            Transform t = transform.Find(prt.name);
            if (t != null) {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
                prt.originalColor = prt.mat.color; // Запоминаем родной цвет
            }
        }
    }

    public override void TakeDamage(GameObject goHit, WeaponType weaponType) {
        Part prtHit = FindPart(goHit.name);
        
        if (prtHit == null && parts.Length > 0) prtHit = parts[0];
        if (prtHit == null || prtHit.health <= 0) return;

        if (prtHit.protectedBy != null) {
            foreach (string s in prtHit.protectedBy) {
                if (!IsPartDestroyed(s)) return; 
            }
        }

        prtHit.health -= Main.GetWeaponDefinition(weaponType).damageOnHit;
        ShowPartDamage(prtHit);

        if (prtHit.health <= 0) prtHit.go.SetActive(false);

        CheckLifeStatus();
    }

    private void ShowPartDamage(Part p) {
        p.mat.color = Color.red;
        Invoke(nameof(ResetPartColors), showDamageDuration);
    }

    private void ResetPartColors() {
        foreach(Part p in parts) {
            if (p.mat != null) p.mat.color = p.originalColor; // Возвращаем родной цвет, а не белый!
        }
    }

    private void CheckLifeStatus() {
        foreach (Part p in parts) if (p.health > 0) return;

        if (!notifiedOfDestruction) {
            Main.S.ShipDestroyed(this);
            notifiedOfDestruction = true;
            Destroy(gameObject);
        }
    }

    Part FindPart(string n) {
        foreach (Part prt in parts) if (prt.name == n) return prt;
        return null;
    }

    bool IsPartDestroyed(string n) {
        Part p = FindPart(n);
        return (p == null || p.health <= 0);
    }
}
