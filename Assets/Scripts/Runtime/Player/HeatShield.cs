using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HeatShield : MonoBehaviour
{
    #region FIELDS

    public float maxShield = 100f;
    public float currentShield;

    public UnityEvent<float> OnShieldChanged;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        currentShield = maxShield;

        OnShieldChanged?.Invoke(currentShield);
    }

    #endregion UNITY METHODS

    #region METHODS

    public void TakeShieldDamage(float damage)
    {
        currentShield -= damage;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);

        OnShieldChanged?.Invoke(currentShield);

        if (currentShield <= 0)
        {
            currentShield = 0;
            Debug.Log("Player died");
            SceneManager.LoadScene(0);
        }
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

    public float GetShieldPercentage()
    {
        return currentShield / maxShield;
    }

    public void ResetShield()
    {
        currentShield = maxShield;
        OnShieldChanged?.Invoke(currentShield);
    }

    #endregion METHODS
}