using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class DiverControllerPrototype : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 180f;

    [Header("Kick Settings")]
    public float kickForce = 5f;
    public float kickDuration = 0.5f;
    public float kickCooldown = 0.5f;
    public AnimationCurve kickCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Buoyancy Settings")]
    public float bcdForce = 3f;
    public float surfaceY = 0f; // Adjustable "sea level"
    public float pressureMultiplier = 0.1f;
    public float maxVerticalSpeed = 2f;
    public float neutralTopY = -5f; // Top level where you are neutral with no air
    public float neutralBottomY = -20f; // Bottom level where you need full BCD to be neutral
    public float maxBcdAir = 100f; // Maximum BCD air amount

    private float currentBcdAir = 0f; // Current BCD air amount
    private Rigidbody rb;
    private Coroutine kickRoutine;
    private bool isCursorLocked = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        RotateTowardMouse();

        if (Input.GetMouseButtonDown(1))
        {
            isCursorLocked = !isCursorLocked;
            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isCursorLocked;
        }

        if (Input.GetKeyDown(KeyCode.W) && kickRoutine == null)
        {
            kickRoutine = StartCoroutine(KickCoroutine());
        }
    }

    private void FixedUpdate()
    {
        HandleBuoyancy();
    }

    // Rotates the diver to face the mouse cursor (in 2D, circular side-on)
    private void RotateTowardMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // Applies forward thrust over time, like a diver's kick
    private IEnumerator KickCoroutine()
    {
        while (Input.GetKey(KeyCode.W))
        {
            float elapsed = 0f;
            while (elapsed < kickDuration)
            {
                float forceMultiplier = kickCurve.Evaluate(elapsed / kickDuration);
                Vector3 force = transform.up * kickForce * forceMultiplier;

                rb.AddForce(force, ForceMode.Acceleration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(kickCooldown);
        }

        kickRoutine = null;
    }

    // Adjusts vertical position based on BCD and pressure at depth
    private void HandleBuoyancy()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentBcdAir = Mathf.Min(currentBcdAir + Time.deltaTime * 10f, maxBcdAir);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentBcdAir = Mathf.Max(currentBcdAir - Time.deltaTime * 10f, 0f);
        }

        float depth = transform.position.y;
        float pressure = Mathf.Lerp(1f, 1f + pressureMultiplier, Mathf.InverseLerp(neutralTopY, neutralBottomY, depth));

        // Apply buoyancy force based on current BCD air amount and pressure
        float buoyancyForce = CalculateBuoyancyForce(depth, pressure);
        rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Acceleration);
    }

    // Calculates the buoyancy force based on the diver's depth and pressure
    private float CalculateBuoyancyForce(float depth, float pressure)
    {
        float bcdEffectiveness = currentBcdAir / maxBcdAir;
        float adjustedBcdForce = bcdForce * bcdEffectiveness / pressure;

        if (depth > neutralTopY)
        {
            return -adjustedBcdForce; // Negative buoyancy above neutral top
        }
        else if (depth < neutralBottomY)
        {
            return adjustedBcdForce; // Positive buoyancy below neutral bottom
        }
        else
        {
            // Interpolate buoyancy force within the neutral range
            float t = Mathf.InverseLerp(neutralBottomY, neutralTopY, depth);
            return Mathf.Lerp(adjustedBcdForce, -adjustedBcdForce, t);
        }
    }

#if UNITY_EDITOR

    // Debug Gizmos for inspector visualization
    private void OnDrawGizmos()
    {
        // Rotation direction
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.up);

        // Surface line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-100f, surfaceY, 0f), new Vector3(100f, surfaceY, 0f));

        // Neutral buoyancy range
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-100f, neutralTopY, 0f), new Vector3(100f, neutralTopY, 0f));
        Gizmos.DrawLine(new Vector3(-100f, neutralBottomY, 0f), new Vector3(100f, neutralBottomY, 0f));
    }

#endif
}
