using UnityEngine;

public class VentWarmer : MonoBehaviour
{
    #region FIELDS

    public float WarmingRate = 75f;

    // Public variable to track if the vent is triggered
    public bool isTriggered = false;

    #endregion FIELDS

    #region UNITY METHODS

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = true; // Set isTriggered to true when the player is in the trigger

            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                // Apply constant temperature increase
                playerManager.playerTemp.AddTemperature(WarmingRate * Time.deltaTime);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = false; // Reset isTriggered to false when the player exits the trigger
        }
    }

    #endregion UNITY METHODS
}