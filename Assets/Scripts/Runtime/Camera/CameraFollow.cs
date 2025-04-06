using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region FIELDS
    public Transform Player;
    public float CamOffset = -6f;
    private Vector3 CameraOffsetTarget;

    #endregion FIELDS

    #region UNITY METHODS
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        CameraOffsetTarget = Player.position;
        
    }
    private void Update()
    {
        if (Player != null)
        {
            CameraOffsetTarget = Player.position;
            CameraOffsetTarget.z = CamOffset;
            transform.position = Vector3.Lerp(transform.position, CameraOffsetTarget, Time.deltaTime * 2f);
        }
    }
    #endregion UNITY METHODS
}
