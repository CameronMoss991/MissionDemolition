using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameMode { idle, playing, levelEnd}
public class MissionDemolition : MonoBehaviour {
    static private MissionDemolition S; // a private Singleton 

    [Header ("Inscribed")]
    public TextMeshProUGUI uitLevel;
    // The UIText_Level Text
    public TextMeshProUGUI uitShots;
    // The UIText_Shots Text
    public Vector3 castlePos; // The place to put castles castles;
    public GameObject[] castles;// An array of the castles
    
    [Header ("Dynamic" )] 
    public int level; 
    public int levelMax; // The current level, The number of levels
    public int shotsTaken; 
    public int maxShotsPerLevel = 10; // The limit you requested
    public GameObject castle;// The current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot"; // FollowCam mode


    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    void Start () {
        S = this; // Define the Singleton
        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        gameOverPanel.SetActive(false); // Make sure it's hidden at start
        StartLevel();
    }

    void StartLevel () {
        // Get rid of the old castle if one exists
        if (castle != null) {
            Destroy( castle ) ;
        }
        // Destroy old projectiles if they exist (the method is not yet writte)
        Projectile.DESTROY_PROJECTILES(); // This will be underlined in red 1/ d

        // Instantiate the new castle
        castle = Instantiate<GameObject>(castles[level] );
        castle.transform.position = castlePos;

        // Reset the goal
        shotsTaken = 0;
        Goal.goalMet = false;

        UpdateGUI();
        mode = GameMode.playing;
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI () {
        // Show the data in the GUITexts
        if (uitLevel != null && uitShots != null) {
            uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
            uitShots.text = "Shots Taken: " + shotsTaken;

            uitShots.text = $"Shots: {shotsTaken} / {maxShotsPerLevel}";
        }
    }

    void Update() {
        UpdateGUI () ;
        // Check for Win Condition
        if ( (mode == GameMode.playing) && Goal.goalMet ) {
        // Change mode to stop checking for level end
        mode = GameMode.levelEnd;
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
        // Start the next level in 2 seconds
        Invoke( "NextLevel", 2f) ;
        }

        // 2. Check for LOSS condition (Over 10 shots)
        if (mode == GameMode.playing && shotsTaken >= maxShotsPerLevel) {
            TriggerGameOver("Restart");
        }

    }

    void NextLevel () {
        level++;
        if (level == levelMax) {
            TriggerGameOver("Victory! All Castles Destroyed!");
            }
            StartLevel ();
    }

    void TriggerGameOver(string message) {
        mode = GameMode.levelEnd;
        gameOverPanel.SetActive(true);
        if (gameOverText != null) {
            gameOverText.text = message;
        }
    }
    // Call this from your "Play Again" Button
    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Static method that allows code anywhere to increment shotsTaken
    static public void SHOT_FIRED() {
        S.shotsTaken++;
    }

    // Static method that allows code anywhere to get a reference to S.castle
    static public GameObject GET_CASTLE(){
        return S. castle;
    }
}