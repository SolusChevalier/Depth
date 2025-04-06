using UnityEngine;

public class VentWarmer : MonoBehaviour
{
    #region FIELDS

    [SerializeField] private float warmth = 0.5f;
    [SerializeField] private float warmthDist = 10f;
    [SerializeField] private SphereCollider sphereCollider;
    private float _timeSinceLastTick = 0f;
    private const float tickInterval = 1f;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            warmthDist = sphereCollider.radius;
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
                if (_timeSinceLastTick >= tickInterval)
                {
                    float distToPlayer = Vector3.Distance(other.transform.position, transform.position);
                    float distModifier = Mathf.Clamp01((warmthDist - distToPlayer) / warmthDist);
                    float adjustedWarmth = warmth * distModifier;

                    playerManager.playerTemp.AddTemperature(adjustedWarmth);
                    _timeSinceLastTick = 0f;
                }
            }
        }
    }

    #endregion UNITY METHODS
}