using UnityEngine;
using UnityEngine.Events;

public struct PlayerState
{
    public float maxShield;
    public float currentShield;
    public float maxTemp;
    public float currentTemp;

    public UnityEvent<float> OnShieldChanged;
    public UnityEvent<float> OnTempChanged;

    public PlayerState(float maxShield, float maxTemp)
    {
        this.maxShield = maxShield;
        this.currentShield = maxShield;
        this.maxTemp = maxTemp;
        this.currentTemp = maxTemp / 2;

        OnShieldChanged = new UnityEvent<float>();
        OnTempChanged = new UnityEvent<float>();
    }

    public void TakeShieldDamage(float damage)
    {
        currentShield -= damage;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        OnShieldChanged?.Invoke(currentShield);
    }

    public void AddShield(float amount)
    {
        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        OnShieldChanged?.Invoke(currentShield);
    }

    public void RemoveShield(float amount)
    {
        currentShield -= amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        OnShieldChanged?.Invoke(currentShield);
    }

    public void RemoveTemperature(float temp)
    {
        currentTemp -= temp;
        currentTemp = Mathf.Clamp(currentTemp, 0, maxTemp);
        OnTempChanged?.Invoke(currentTemp);
    }

    public void AddTemperature(float temp)
    {
        currentTemp += temp;
        currentTemp = Mathf.Clamp(currentTemp, 0, maxTemp);
        OnTempChanged?.Invoke(currentTemp);
    }

    public float GetShieldPercentage()
    {
        return currentShield / maxShield;
    }

    public float GetTemperaturePercentage()
    {
        return currentTemp / maxTemp;
    }

    public void ResetShield()
    {
        currentShield = maxShield;
        OnShieldChanged?.Invoke(currentShield);
    }

    public void ResetTemperature()
    {
        currentTemp = maxTemp;
        OnTempChanged?.Invoke(currentTemp);
    }
}