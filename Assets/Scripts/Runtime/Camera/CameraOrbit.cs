using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    #region FIELDS

    [SerializeField]
    private Transform focus = default;

    [SerializeField, Range(1f, 20f)]
    private float distance = 5f;

    public float minDistance = 1f, maxDistance = 20f;

    public float scrollRate = 0.1f;

    [SerializeField, Min(0f)]
    private float focusRadius = 5f;

    [SerializeField, Range(0f, 1f)]
    private float focusCentering = 0.5f;

    [SerializeField, Range(1f, 360f)]
    private float rotationSpeed = 90f;

    [SerializeField, Range(-89f, 89f)]
    private float minVerticalAngle = -45f, maxVerticalAngle = 45f;

    public float TmpMinVerticalAngle, TmpMaxVerticalAngle;

    [SerializeField, Min(0f)]
    private float alignDelay = 5f;

    [SerializeField, Range(0f, 90f)]
    private float alignSmoothRange = 45f;

    [SerializeField]
    private LayerMask obstructionMask = -1;

    private Camera regularCamera;

    private Vector3 focusPoint, previousFocusPoint;

    public Vector2 orbitAngles = new Vector2(45f, 0f);

    private float lastManualRotationTime;
    public Vector3 CamDirFlattened => Vector3.ProjectOnPlane(transform.forward, focus.up);

    //public HoleManager HoleManager;

    private Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y =
                regularCamera.nearClipPlane *
                Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    #endregion FIELDS

    #region UNITY METHODS

    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    private void Awake()
    {
        TmpMinVerticalAngle = minVerticalAngle;
        TmpMaxVerticalAngle = maxVerticalAngle;
        regularCamera = GetComponent<Camera>();
        focus = GameObject.FindGameObjectWithTag("Player").transform;
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        //if (HoleManager.GetPause())
        //return;

        UpdateFocusPoint();
        Quaternion lookRotation = transform.localRotation;
        // Hide and lock cursor when right mouse button pressed
        /*if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }*/

        // Unlock and show cursor when right mouse button released
        /*if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }*/
        if (Input.GetMouseButtonUp(0))
        {
            minVerticalAngle = TmpMinVerticalAngle;
            maxVerticalAngle = TmpMaxVerticalAngle;
        }
        if (Input.GetMouseButtonDown(0))
        {
            minVerticalAngle = transform.localEulerAngles.x;
            maxVerticalAngle = transform.localEulerAngles.x;
        }

        // Rotation
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        /*if (Input.GetMouseButton(1))
        {
            if (ManualRotation() || AutomaticRotation())
            {
                ConstrainAngles();
                lookRotation = Quaternion.Euler(orbitAngles);
            }
        }*/
        else
        {
            //lookRotation = transform.localRotation;
        }

        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;

        /*Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        if (Physics.BoxCast(
            castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
            lookRotation, castDistance, obstructionMask
        ))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }*/

        transform.SetPositionAndRotation(lookPosition, lookRotation);
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            distance -= scroll * scrollRate;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    #endregion UNITY METHODS

    #region METHODS

    private void UpdateFocusPoint()
    {
        previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            float t = 1f;
            if (distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }
            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else
        {
            focusPoint = targetPoint;
        }
    }

    private bool ManualRotation()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X")
        );
        const float e = 0.01f;
        if (input.x < -e || input.x > e || input.y < -e || input.y > e)
        {
            input = new Vector2(-input.x, input.y);
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }

    private bool AutomaticRotation()
    {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay)
        {
            return false;
        }

        Vector2 movement = new Vector2(
            focusPoint.x - previousFocusPoint.x,
            focusPoint.z - previousFocusPoint.z
        );
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.0001f)
        {
            return false;
        }

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        float rotationChange =
            rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if (180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }
        orbitAngles.y =
            Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    private void ConstrainAngles()
    {
        orbitAngles.x =
            Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        /*if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }*/
    }

    private static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

    #endregion METHODS
}