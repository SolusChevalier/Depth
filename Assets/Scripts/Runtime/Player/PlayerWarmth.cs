using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerWarmth : MonoBehaviour
{
    #region FIELDS

    public float maxTemp = 100f;
    public float currentTemp;
    public GameObject player;

    public UnityEvent<float> OnTempChanged;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        currentTemp = maxTemp/2;

        OnTempChanged?.Invoke(currentTemp);
    }



    #endregion UNITY METHODS

    #region METHODS

    public void RemoveWarmth(int Temp)
    {
        currentTemp -= Temp;
        currentTemp = Mathf.Clamp(currentTemp, 0, maxTemp);

        OnTempChanged?.Invoke(currentTemp);

        if (currentTemp <= 0)
        {
            /*currentWarmth = 0;
            Debug.Log("Player died");
            SceneManager.LoadScene(0);*/
            //Mabye die?
        }
    }

    public void AddWarmth(int warmth)
    {
        currentTemp += warmth;
        currentTemp = Mathf.Clamp(currentTemp, 0, maxTemp);  // Ensure health stays within bounds

        OnTempChanged?.Invoke(currentTemp);
    }
    public void ResetWarmth()
    {
        currentTemp = maxTemp;
        OnTempChanged?.Invoke(currentTemp);
    }


    #endregion METHODS
}
