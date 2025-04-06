using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region FIELDS

    public HeatShield heatShield;
    public PlayerTemperature playerTemp;
    private float damageRate = 1.0f;
    private float minSafeTemp = 0.4f;
    private float maxSafeTemp = 0.6f;
    public float tempDecayRate = 0.01f;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        heatShield = GetComponent<HeatShield>();
        playerTemp = GetComponent<PlayerTemperature>();
    }

    private void FixedUpdate()
    {
        ApplyHealthDamageBasedOnWarmth();
        //ApplyTemperatureDecay();
    }

    #endregion UNITY METHODS

    #region METHODS

    private void ApplyHealthDamageBasedOnWarmth()
    {
        if (playerTemp != null && heatShield != null)
        {
            float warmthLevel = playerTemp.currentTemp;
            float damage = CalculateDamage(warmthLevel);
            heatShield.TakeShieldDamage(Mathf.RoundToInt(damage * Time.fixedDeltaTime));
        }
    }

    private void ApplyTemperatureDecay()
    {
        if (playerTemp != null)
        {
            playerTemp.RemoveTemperature(tempDecayRate * Time.fixedDeltaTime);
        }
    }

    public void SetMaxHeatShield(float newShield)
    {
        heatShield.maxShield = newShield;
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