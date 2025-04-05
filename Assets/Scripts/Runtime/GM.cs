using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    #region FIELDS

    public PlayerManager playerManager;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        if (playerManager == null)
        {
            playerManager = FindFirstObjectByType<PlayerManager>();
        }   
    }

    #endregion UNITY METHODS

    #region METHODS


    #endregion METHODS
}