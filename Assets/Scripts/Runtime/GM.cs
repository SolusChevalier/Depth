using UnityEngine;

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
}