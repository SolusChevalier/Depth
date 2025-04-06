using UnityEngine;

public class VentWarmer : MonoBehaviour
{
    #region FIELDS

    public float warmth = 0.5f;
    public float warmthDist = 10f;
    public Transform ventCenter;

    #endregion FIELDS

    #region UNITY METHODS

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                float distToPlayer = Vector3.Distance(other.transform.position, ventCenter.position);
                float distModifier = Mathf.Clamp01((warmthDist - distToPlayer) / warmthDist);
                float adjustedWarmth = warmth * distModifier;

                playerManager.playerTemp.AddTemperature(adjustedWarmth * Time.deltaTime);
            }
        }
    }

    #endregion UNITY METHODS
}