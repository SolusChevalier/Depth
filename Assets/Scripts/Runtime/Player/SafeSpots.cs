using UnityEngine;

public class SafeSpots : MonoBehaviour
{
    #region FIELDS

    public PlayerManager playerManager;
    public float safeSpotRadius = 5f;
    public float ShieldIncreaseRate = 1f;
    public int NewMaxHeatShield = 100;
    public bool isInSafeSpot = false;
    private float _timeSinceLastTick = 0f;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        playerManager = FindFirstObjectByType<PlayerManager>();
        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider != null)
        {
            safeSpotRadius = collider.radius;
        }
    }

    //without using flag
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag.Equals("Player"))
        {
            if (isInSafeSpot)
            {
                _timeSinceLastTick += Time.fixedDeltaTime;
                if (_timeSinceLastTick >= 1f)
                {
                    HeatShield heatShield = GetComponent<HeatShield>();
                    if (heatShield != null)
                    {
                        playerManager.SetMasHeatShield(NewMaxHeatShield);
                        _timeSinceLastTick = 0f; // Reset the timer
                    }
                }
            }
        }
    }

    //using flag
    /*
    public void FixedUpdate()
    {
        if (isInSafeSpot)
        {
            _timeSinceLastTick += Time.fixedDeltaTime;
            if (_timeSinceLastTick >= 1f)
            {
                HeatShield heatShield = GetComponent<HeatShield>();
                if (heatShield != null)
                {
                    playerManager.SetMasHeatShield(NewMaxHeatShield);
                    _timeSinceLastTick = 0f; // Reset the timer
                }
            }
        }
    }*/

    #endregion UNITY METHODS
}