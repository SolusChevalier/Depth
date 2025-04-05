using UnityEngine;

public class VentWarmer : MonoBehaviour
{
    #region FIELDS
    public float Wartmth = 0.5f;
    public float WarmthDist = 10f;
    public Transform VentCenter;
    #endregion FIELDS
    #region UNITY METHODS

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float distToPlayer = Vector3.Distance(other.transform.position, VentCenter.position);
            float distModifier = Mathf.Clamp01((WarmthDist - distToPlayer) / WarmthDist);

        }
    }
    #endregion UNITY METHODS
}
