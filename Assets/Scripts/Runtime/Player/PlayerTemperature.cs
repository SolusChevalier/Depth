using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerTemperature : MonoBehaviour
{
    #region FIELDS

    public float maxTemp = 100f;
    public float currentTemp;

    public UnityEvent<float> OnTempChanged;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        currentTemp = maxTemp;

        OnTempChanged?.Invoke(currentTemp);
    }

    #endregion UNITY METHODS

    #region METHODS

    public void RemoveTemperature(float temp)
    {
        currentTemp -= temp;
        currentTemp = Mathf.Clamp(currentTemp, 0, maxTemp);

        OnTempChanged?.Invoke(currentTemp);

        if (currentTemp <= 0)
        {
            currentTemp = 0;
            Debug.Log("Player died");
            SceneManager.LoadScene(2);
        }
    }

    public void AddTemperature(float temp)
    {
        currentTemp += temp;
        currentTemp = Mathf.Clamp(currentTemp, 0, maxTemp);

        OnTempChanged?.Invoke(currentTemp);
    }

    public float GetTemperaturePercentage()
    {
        return currentTemp / maxTemp;
    }

    public void ResetTemperature()
    {
        currentTemp = maxTemp;
        OnTempChanged?.Invoke(currentTemp);
    }

    #endregion METHODS
}