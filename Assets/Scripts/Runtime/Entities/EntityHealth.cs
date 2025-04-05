using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EntityHealth : MonoBehaviour
{
    #region FIELDS

    public int maxHealth = 100;
    public int currentHealth;
    public GameObject player;

    // UnityEvent that will be invoked when health changes
    public UnityEvent<int> OnHealthChanged;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth);
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Ensure health stays within bounds

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player died");
            SceneManager.LoadScene(0);
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Ensure health stays within bounds

        OnHealthChanged?.Invoke(currentHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    #endregion METHODS
}