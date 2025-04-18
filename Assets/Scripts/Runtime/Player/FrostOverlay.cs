using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FrostOverlay : MonoBehaviour
{
    public Image FrostOverLayUI;
    public PlayerTemperature playerTemp;
    public float maxTemp = 60f;
    public float minTemp = 0f;

    // Update is called once per frame
    private void Update()
    {
        float currTemp = playerTemp.currentTemp;

        float normTemp = Mathf.InverseLerp(minTemp, maxTemp, currTemp);//find value between 0 and 1 to translate to transpareny of ovelay
        float alpha = 1f - normTemp;

        Color frostColor = FrostOverLayUI.color;
        frostColor.a = Mathf.Lerp(0f, 1f, alpha);
        FrostOverLayUI.color = frostColor;
    }
}