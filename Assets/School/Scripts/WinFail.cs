using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WinFail : MonoBehaviour
{
    public Transform dropZone; // The drop zone where the player drops evidences
    public float detectionRadius = 5f; // Radius around the dropZone to detect evidences
    public KeyCode pressKey = KeyCode.O; // Key to trigger the check when near the drop zone
    private bool playerInRange = false; // Whether the player is near the drop zone
    private int totalEvidences; // Total number of evidences in the scene

   

    public float timer = 180f; // 5 minutes in seconds
    private bool gameOver = false; // Game over state

    public TextMeshProUGUI timerText; // UI Text to show the timer

    public GameObject winPanel; // Assign your Win UI panel in the Inspector
    public GameObject failPanel; // Assign your Win UI panel in the Inspector
    public GameObject blurPanel; // The Full-Screen Blur Panel


    // Start is called before the first frame update
    void Start()
    {
      

        gameOver = false;

        // Count the total number of evidences in the scene
        totalEvidences = GameObject.FindGameObjectsWithTag("object").Length;
        Debug.Log("Total evidences to collect: " + totalEvidences);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver) return; // Stop game logic if game over

        // Countdown timer
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            GameOver(); // Game over if time runs out
        }

        // Update timer display
        UpdateTimerDisplay();


        // Check if the player is within range of the drop zone
        float distance = Vector3.Distance(transform.position, dropZone.position);
        if (distance <= detectionRadius)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        // If the player is near the drop zone and presses the "O" key
        if (playerInRange && Input.GetKeyDown(pressKey))
        {
            CheckEvidences();
        }
    }

    void CheckEvidences()
    {
        // Find all evidences and count how many are near the drop zone
        GameObject[] evidences = GameObject.FindGameObjectsWithTag("object");
        int evidencesNearDropZone = 1;

        foreach (GameObject evidence in evidences)
        {
            float distance = Vector3.Distance(evidence.transform.position, dropZone.position);
            if (distance <= detectionRadius)
            {
                evidencesNearDropZone++;
            }
        }

        // Check if all evidences are in the drop zone
        if (evidencesNearDropZone == totalEvidences)
        {
            Debug.Log("You win! All evidences have been dropped at the drop zone.");

            blurPanel.SetActive(true); // Enable blur effect
            winPanel.SetActive(true); // Show the Win panel
        }
        else
        {
            Debug.Log("Not all evidences are dropped. Keep trying!");
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the detection radius in the scene view
        if (dropZone != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(dropZone.position, detectionRadius);
        }
    }

    void UpdateTimerDisplay()
    {
        // Convert timer to minutes and seconds
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        // Update the UI text to show the timer
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void GameOver()
    {
        Debug.Log("Game Over! Time's up!");
        gameOver = true; // Stop the game if time's up
        blurPanel.SetActive(true); // Enable blur effect
        failPanel.SetActive(true); // Show the Win panel
    }
}
