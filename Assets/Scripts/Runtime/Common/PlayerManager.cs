using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region FIELDS
    private EntityHealth playerHealth;
    private PlayerWarmth playerTemp;
    private float damageRate = 1.0f;
    private float minSafeTemp = 0.4f; 
    private float maxSafeTemp = 0.6f;
    #endregion FIELDS

    #region UNITY METHODS
    private void Start()
    {
        playerHealth = GetComponent<EntityHealth>();
        playerTemp = GetComponent<PlayerWarmth>();
    }

    private void FixedUpdate()
    {
        ApplyHealthDamageBasedOnWarmth();
    }
    #endregion UNITY METHODS

    #region METHODS
    private void ApplyHealthDamageBasedOnWarmth()
    {
        if (playerTemp != null && playerHealth != null)
        {
            float warmthLevel = playerTemp.currentTemp;
            float damage = CalculateDamage(warmthLevel);
            playerHealth.TakeDamage(Mathf.RoundToInt(damage * Time.fixedDeltaTime));
        }
    }

    private float CalculateDamage(float warmthLevel)
    {
        if (warmthLevel < minSafeTemp)
        {
            
            return damageRate * (minSafeTemp - warmthLevel) / minSafeTemp;
        }
        else if (warmthLevel > maxSafeTemp)
        {
            
            return damageRate * (warmthLevel - maxSafeTemp) / (1.0f - maxSafeTemp);
        }
        else
        {
            
            return 0;
        }
    }
    #endregion METHODS
}
