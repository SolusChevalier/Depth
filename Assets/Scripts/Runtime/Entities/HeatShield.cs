using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HeatShield : MonoBehaviour
{
    #region FIELDS

    public int MaxShield = 100;
    public int CurrentShield;
    public GameObject player;

    // UnityEvent that will be invoked when health changes
    public UnityEvent<int> OnShieldChanged;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        CurrentShield = MaxShield;

        OnShieldChanged?.Invoke(CurrentShield);
    }

    private void Update()
    {
        if (player.transform.position.y <= -25) // If the player falls off the map
        {
            Debug.Log("Player fell off the map" + player.transform.position.y);
            SceneManager.LoadScene(0);
        }
    }

    #endregion UNITY METHODS

    #region METHODS

    public void TakeShieldDamage(int damage)
    {
        CurrentShield -= damage;
        CurrentShield = Mathf.Clamp(CurrentShield, 0, MaxShield);  // Ensure health stays within bounds

        OnShieldChanged?.Invoke(CurrentShield);

        if (CurrentShield <= 0)
        {
            CurrentShield = 0;
            Debug.Log("Player died");
            SceneManager.LoadScene(0);
        }
    }

    public void AddShield(int Amount)
    {
        CurrentShield += Amount;
        CurrentShield = Mathf.Clamp(CurrentShield, 0, MaxShield);  // Ensure health stays within bounds

        OnShieldChanged?.Invoke(CurrentShield);
    }

    public void ResetShield()
    {
        CurrentShield = MaxShield;
        OnShieldChanged?.Invoke(CurrentShield);
    }

    #endregion METHODS
}