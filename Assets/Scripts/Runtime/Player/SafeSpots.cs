using UnityEngine;

public class SafeSpots : MonoBehaviour
{
    #region FIELDS

    [SerializeField] private float safeSpotRadius = 5f;
    [SerializeField] private SphereCollider SphereCollider;
    public float shieldIncreaseRate = 1f;
    public int newMaxHeatShield = 100;
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
                    playerManager.SetMaxHeatShield(newMaxHeatShield);
                    playerManager.heatShield.AddShield(shieldIncreaseRate);
                    _timeSinceLastTick = 0f;
                }
            }
        }
    }

    #endregion UNITY METHODS
}