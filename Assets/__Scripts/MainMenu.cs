using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controls the Main Menu. Displays the score from the previous game session.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreDisplay; // Link your text object here

    void Start() {
        // Load the last achieved score from memory
        int lastPoints = PlayerPrefs.GetInt("LastScore", 0);
        
        if (scoreDisplay != null) {
            scoreDisplay.text = "LAST SCORE: " + lastPoints.ToString();
        } 
    }

    /// <summary>
    /// Starts the game with chosen difficulty parameters.
    /// </summary>
    public void SelectDifficulty(float spawnRate, int sceneIndex, int targetScore) {
        Main.score = 0;
        Main.lives = 3;

        PlayerPrefs.SetFloat("EnemySpawnRate", spawnRate);
        PlayerPrefs.SetInt("TargetScore", targetScore);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(sceneIndex); 
    }

    // Button Links
    public void PlayEasy()   => SelectDifficulty(0.8f, 1, 1000); 
    public void PlayNormal() => SelectDifficulty(1.8f, 2, 3000); 
    public void PlayHard()   => SelectDifficulty(3.3f, 3, 5000); 
}
