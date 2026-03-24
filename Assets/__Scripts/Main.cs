using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; 

/// <summary>
/// Central Game Manager. 
/// Handles scoring, life tracking, enemy spawning, and scene transitions.
/// Implemented as a Singleton with 'DontDestroyOnLoad' to maintain game state across levels.
/// </summary>
public class Main : MonoBehaviour
{
    static public Main S; // Singleton instance
    static private Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed: Level Settings")]
    public int scoreToNextLevel = 1000;
    public float enemySpawnPerSecond = 0.5f; 
    public float gameRestartDelay = 2f;
    public GameObject[] prefabEnemies;      
    public WeaponDefinition[] weaponDefinitions;
    public GameObject[] powerUpPrefabs;

    [Header("Dynamic State (Static)")]
    static public int score = 0; 
    static public int lives = 3; 
    
    [Header("UI References")]
    public TextMeshProUGUI scoreGT; 
    public TextMeshProUGUI livesGT;

    private bool isLevelEnding = false;
    private BoundsCheck bndCheck;

    void Awake()
    {
        // Singleton pattern: ensures only one Main exists and persists between scenes
        if (S == null) {
            S = this;
            DontDestroyOnLoad(gameObject); 
        } else {
            Destroy(gameObject);
            return;
        }

        bndCheck = GetComponent<BoundsCheck>();
        // Pre-cache weapon definitions into a Dictionary for fast lookup
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions) {
            WEAP_DICT[def.type] = def;
        }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    /// <summary>
    /// Triggered whenever a scene is loaded. 
    /// Restores UI links and restarts difficulty-based logic.
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        FindUIElements();
        isLevelEnding = false; 

        // Read difficulty settings from PlayerPrefs (passed from the Menu)
        if (PlayerPrefs.HasKey("EnemySpawnRate")) {
            enemySpawnPerSecond = PlayerPrefs.GetFloat("EnemySpawnRate");
        }
        
        // Initialize target score only at the start of a session
        if (score == 0 && PlayerPrefs.HasKey("TargetScore")) {
            scoreToNextLevel = PlayerPrefs.GetInt("TargetScore");
        }
        // Restart enemy spawning loop for the new stage
        CancelInvoke(nameof(SpawnEnemy));
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    /// <summary>
    /// Robust search for UI Text components in the current scene hierarchy.
    /// Necessary for 'DontDestroyOnLoad' objects to find new scene objects.
    /// </summary>
    public void FindUIElements() {
        TextMeshProUGUI[] allTexts = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        scoreGT = null; livesGT = null;
        foreach (var t in allTexts) {
            string tName = t.gameObject.name.ToLower().Trim();
            if (tName == "score_text") scoreGT = t;
            if (tName == "health_text" || tName == "lives_text") livesGT = t;
        }
        UpdateGUI();
    }

    void Update() {
        // Progression check: triggers level completion
        if (!isLevelEnding && score >= scoreToNextLevel) {
            isLevelEnding = true;
            Invoke(nameof(NextLevel), 1f);
        }
    }

    /// <summary>
    /// Updates the on-screen text labels with current stats.
    /// </summary>
    public void UpdateGUI() {
        if (scoreGT != null) scoreGT.text = "Score: " + score;
        if (livesGT != null) livesGT.text = "Lives: " + lives;
    }

    /// <summary>
    /// Spawns a random enemy from the prefab list above the screen bounds.
    /// </summary>
    public void SpawnEnemy() {
        if (isLevelEnding || prefabEnemies.Length == 0) return;
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // Dynamically calculate camera bounds for consistent spawning
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight*Camera.main.aspect;

        float padding = 2f;
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(-bndCheck.camWidth + padding, bndCheck.camWidth - padding);
        pos.y = bndCheck.camHeight + padding;
        go.transform.position = pos;

        // Recursive call to maintain the spawning loop
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    /// <summary>
    /// Called upon enemy destruction to award score and potentially drop loot.
    /// </summary>
    public void ShipDestroyed(Enemy e) {
        if (isLevelEnding) return;

        // PowerUp loot table logic
        if (powerUpPrefabs != null && powerUpPrefabs.Length > 0) {
            if (Random.value <= e.powerUpDropChance) {
                int ndx = Random.Range(0, powerUpPrefabs.Length);
                GameObject go = Instantiate(powerUpPrefabs[ndx]);
                go.transform.position = e.transform.position;
            }
        }

        score += e.score; 
        UpdateGUI();
    }
    public void HeroDied() {
        lives--;
        UpdateGUI();
        
        if (lives > 0) {
            Invoke(nameof(RestartLevel), gameRestartDelay);
        } else {
            FinalizeGame();
        }
    }

    private void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    
    private void NextLevel() {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings) {
            // Successive difficulty scaling
            scoreToNextLevel += 2000;
            SceneManager.LoadScene(nextIndex);
        } else {
            FinalizeGame();
        }
    }

    /// <summary>
    /// Saves Highscore and resets static data before returning to the Menu.
    /// </summary>
    private void FinalizeGame() {
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.Save();

        // Data reset for a clean future run
        score = 0; lives = 3;
        SceneManager.LoadScene(0); 
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt) {
        if (WEAP_DICT != null && WEAP_DICT.ContainsKey(wt)) return WEAP_DICT[wt];
        return new WeaponDefinition();
    }
}