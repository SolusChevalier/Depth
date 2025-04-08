using UnityEngine;

public class SafeSpots : MonoBehaviour
{
    #region FIELDS

    public float TempIncrease = 75f;

    // Public variable to track if the safe spot is triggered
    public bool isTriggered = false;

    private PlayerManager playerManager; // Cached reference to PlayerManager

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        // Find the PlayerManager in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerManager = player.GetComponent<PlayerManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerManager != null)
        {
            isTriggered = true; // Set isTriggered to true when the player enters the trigger

            // Set the player as hidden
            playerManager.SetHiddenState(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerManager != null)
        {
            // Apply constant temperature increase
            playerManager.playerTemp.AddTemperature(TempIncrease * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerManager != null)
        {
            isTriggered = false; // Reset isTriggered to false when the player exits the trigger

            // Unhide the player
            playerManager.SetHiddenState(false);
        }
    }

    #endregion UNITY METHODS
}