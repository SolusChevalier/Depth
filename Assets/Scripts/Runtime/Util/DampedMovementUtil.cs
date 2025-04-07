using UnityEngine;

public class DampedMovementUtil : MonoBehaviour
{
    #region FIELDS

    public GameObject target;
    public float Damping = 1;
    public bool HarmonicDampening = false;

    [Tooltip("If negative speed is not limited - for Harmonic Dampening")]
    public float MaxSpeed = 10f;

    public bool useFixedUpdate = false;
    public bool FixRotation = false;
    public bool FixPosition = false;

    private Vector3 _vel = new Vector3();

    #endregion FIELDS

    #region UNITY METHODS

    private void Update()
    {
        if (!useFixedUpdate)
        {
            Move();
        }
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            Move();
        }
    }

    #endregion UNITY METHODS

    #region METHODS

    public void Move()
    {
        if (!FixPosition)
        {
            MoveTo();
        }
        if (!FixRotation)
        {
            RotateToward();
        }
        if (MaxSpeed > 0)
        {
            _vel = Vector3.ClampMagnitude(_vel, MaxSpeed);
        }
    }

    private void RotateToward()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.forward), Time.deltaTime * Damping);
    }

    private void MoveTo()
    {
        if (!HarmonicDampening)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * Damping);
        }
        else
        {
            //_vel = Vector3.ClampMagnitude(_vel, MaxSpeed);

            var n1 = _vel - (transform.position - target.transform.position) * (Mathf.Pow(Damping, 2)) * Time.deltaTime;
            var n2 = 1 + Damping * Time.deltaTime;
            _vel = n1 / (n2 * n2);

            transform.position += _vel * Time.deltaTime;
        }
    }

    #endregion METHODS
}