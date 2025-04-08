using UnityEngine;

public class VentWarmer : MonoBehaviour
{
    #region FIELDS

    public float WarmingRate = 0.5f;
    private float _timeSinceLastTick = 0f;
    private const float tickInterval = 0.8f;

    #endregion FIELDS

    #region UNITY METHODS

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                _timeSinceLastTick += Time.fixedDeltaTime;
                if (_timeSinceLastTick >= tickInterval)
                {
                    playerManager.playerTemp.AddTemperature(WarmingRate * Time.fixedDeltaTime);
                    _timeSinceLastTick = 0f;
                }
            }
        }
    }

    #endregion UNITY METHODS
}