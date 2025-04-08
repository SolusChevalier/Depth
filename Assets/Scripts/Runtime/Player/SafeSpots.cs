using UnityEngine;

public class SafeSpots : MonoBehaviour
{
    #region FIELDS

    [SerializeField] private float safeSpotRadius = 5f;
    [SerializeField] private SphereCollider SphereCollider;
    public float TempIncrease = 1f;
    private float _timeSinceLastTick = 0f;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        SphereCollider = GetComponent<SphereCollider>();
        if (SphereCollider != null)
        {
            safeSpotRadius = SphereCollider.radius;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                _timeSinceLastTick += Time.fixedDeltaTime;
                if (_timeSinceLastTick >= 1f)
                {
                    playerManager.playerTemp.AddTemperature(TempIncrease * Time.fixedDeltaTime);
                    playerManager.SetHiddenState(true); // Hide the player
                    _timeSinceLastTick = 0f;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.SetHiddenState(false); // Unhide the player
            }
        }
    }

    #endregion UNITY METHODS
}