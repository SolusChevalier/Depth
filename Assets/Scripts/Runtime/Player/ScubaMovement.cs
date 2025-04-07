using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ScubaMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float thrustForce = 5f;

    [SerializeField] private Vector2 InputCords = new Vector2(0, 0);
    public float rotationSpeed = 100f;
    public float verticalBuoyancyForce = 3f;
    public float maxVerticalSpeed = 2f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleBuoyancy();
    }

    private void HandleMovement()
    {
        // Forward/Backward
        float moveInput = Input.GetAxis("Vertical"); // W/S
        Vector3 moveForce = transform.up * moveInput * thrustForce;
        rb.AddForce(moveForce);

        // Rotate left/right (yaw)
        float turnInput = Input.GetAxis("Horizontal"); // A/D
        float turn = turnInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 1f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void CalcInput()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X")
        );
        const float e = 0.01f;
        if (input.x < -e || input.x > e || input.y < -e || input.y > e)
        {
            InputCords += Time.unscaledDeltaTime * input;
        }
    }

    private void HandleBuoyancy()
    {
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.LeftShift))
            verticalInput = 1f;
        else if (Input.GetKey(KeyCode.LeftControl))
            verticalInput = -1f;

        if (verticalInput != 0)
        {
            // Apply vertical force (buoyancy)
            Vector3 upwardForce = Vector3.up * verticalInput * verticalBuoyancyForce;

            // Optional clamp to avoid infinite rising/falling
            if (Mathf.Abs(rb.linearVelocity.y) < maxVerticalSpeed)
            {
                rb.AddForce(upwardForce, ForceMode.Acceleration);
            }
        }
    }
}