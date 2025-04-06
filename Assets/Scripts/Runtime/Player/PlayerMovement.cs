using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float swimForce = 10f;

    public float rotationSpeed = 100f;
    public float buoyancyForce = 5f;
    public float maxBuoyancySpeed = 3f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // No gravity in water
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        HandleBuoyancy();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical"); // W/S keys
        Vector3 force = transform.up * moveInput * swimForce;
        rb.AddForce(force);
    }

    private void HandleRotation()
    {
        float rotationInput = -Input.GetAxis("Horizontal"); // A/D keys (inverted for correct direction)
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, 0f, rotationInput * rotationSpeed * Time.fixedDeltaTime));
    }

    private void HandleBuoyancy()
    {
        float verticalSpeed = rb.linearVelocity.y;

        if (Input.GetKey(KeyCode.LeftShift) && verticalSpeed < maxBuoyancySpeed)
        {
            rb.AddForce(Vector3.up * buoyancyForce);
        }
        else if (Input.GetKey(KeyCode.LeftControl) && verticalSpeed > -maxBuoyancySpeed)
        {
            rb.AddForce(Vector3.down * buoyancyForce);
        }
    }
}

/*if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) && Time.time > nextSwimKick)
{
    rb.AddForce(transform.up * swimKickForce, ForceMode.Impulse);
    nextSwimKick = Time.time + kickCooldown;
}*/