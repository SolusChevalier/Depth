using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region FIELDS

    public HeatShield heatShield;
    public PlayerTemperature playerTemp;
    [SerializeField] private float damageRate = 1.0f;
    [SerializeField] private float minSafeTemp = 35f;
    [SerializeField] private float maxSafeTemp = 65f;
    public float tempDecayRate = 0.01f;
    private float _progression = 0.0f;
    [SerializeField] private bool isCursorLocked = false;
    [SerializeField] private float _speedIncrement = 0.1f;

    public bool isHidden = false; // Track if the player is hidden

    // Raycast hiding detection
    public LayerMask hidingLayer; // Layer mask for hiding objects

    public float raycastDistance = 5f; // Maximum distance for the raycast

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        heatShield = GetComponent<HeatShield>();
        playerTemp = GetComponent<PlayerTemperature>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isCursorLocked = !isCursorLocked;
            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isCursorLocked;
        }

        // Perform hiding detection
        //DetectHiding();
    }

    private void FixedUpdate()
    {
        _progression += Time.deltaTime * _speedIncrement;
        ApplyHealthDamageBasedOnWarmth();
        ApplyTemperatureDecay();
    }

    #endregion UNITY METHODS

    #region METHODS

    private void ApplyHealthDamageBasedOnWarmth()
    {
        if (playerTemp != null && heatShield != null)
        {
            float warmthLevel = playerTemp.currentTemp;
            float damage = CalculateDamage(warmthLevel);
            heatShield.TakeShieldDamage(Mathf.RoundToInt(damage * Time.fixedDeltaTime));
        }
    }

    private void ApplyTemperatureDecay()
    {
        if (playerTemp != null)
        {
            playerTemp.RemoveTemperature(tempDecayRate * Time.fixedDeltaTime);
        }
    }

    public void SetHiddenState(bool hidden)
    {
        isHidden = hidden;
        //Debug.Log("Player hidden state: " + isHidden);
    }

    public void SetMaxHeatShield(float newShield)
    {
        heatShield.maxShield = newShield;
    }

    private float CalculateDamage(float warmthLevel)
    {
        if (warmthLevel < minSafeTemp)
        {
            return damageRate * (minSafeTemp - warmthLevel) / minSafeTemp;
        }
        else if (warmthLevel > maxSafeTemp)
        {
            return damageRate * (warmthLevel - maxSafeTemp) / (1.0f - maxSafeTemp);
        }
        else
        {
            return 0;
        }
    }

    /* private void DetectHiding()
     {
         // Perform a raycast from the player's position to check for hiding objects
         Ray ray = new Ray(transform.position, Vector3.up); // Example direction (upward)
         RaycastHit hit;

         if (Physics.Raycast(ray, out hit, raycastDistance, hidingLayer))
         {
             // Check if the raycast hit a hiding object
             if (hit.collider.CompareTag("HidingObject"))
             {
                 SetHiddenState(true);
                 return;
             }
         }

         // If no hiding object is detected, unhide the player
         SetHiddenState(false);
     }*/

    #endregion METHODS
}