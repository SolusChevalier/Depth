using UnityEngine;
using System.Collections;

public class RandomMoveBehavFromRandomGame : MonoBehaviour
{
    #region FIELDS

    public GameObject ball;

    /// <summary>
    /// private bool SliderPower = false;
    /// </summary>
    private Transform _ballTrans;

    private SphereCollider _ballCol;
    public float shotPwr;
    public float MaxShotPwr = 20f;

    //private float _shotPwrMod = 5f;
    //private float _stopSpd = 0.1f;
    public bool _isIdle, _isAiming;

    public GameObject AimingLine;

    public GameObject Spawn;
    private CameraOrbit _camOrbitReference;
    public Vector2 InputCords;
    public Vector2 StartShotMouseCords;
    public Vector3 ForewardVector;

    //private LineRenderer _lineRenderer;
    private Rigidbody rb;

    //private Ray ray;
    public Camera mainCam;

    private Transform camTrans;

    //private GameObject _TempLine;
    //private bool _AimingLineActive = false;

    //public PlayerDataSO playerDataSO;

    //public HoleManager HoleManager;

    #endregion FIELDS

    #region UNITYMETHODS

    private void Awake()
    {
        //mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Time.timeScale = 1;
        camTrans = mainCam.GetComponent<Transform>();
        rb = this.GetComponent<Rigidbody>();
        _ballCol = ball.GetComponent<SphereCollider>();
        _ballTrans = ball.GetComponent<Transform>();
        _isAiming = false;
        _isIdle = false;
        shotPwr = 0;
        //_lineRenderer = AimingLine.GetComponentInChildren<LineRenderer>();
        //_lineRenderer = this.GetComponent<LineRenderer>();
        //_lineRenderer.enabled = false;
        /*SliderPower = playerDataSO.GetSliderPower();                                                      <=================
        playerDataSO.SetMaxShotPwr((int)MaxShotPwr);
        playerDataSO.SetIsAiming(false);
        //_ballTrans.position = Spawn.transform.position;
        HoleManager.Restart();*/
        StartCoroutine(ResetBall(0.1f));
        _camOrbitReference = mainCam.GetComponent<CameraOrbit>();
    }

    private void FixedUpdate()
    {
        /*if (IsGrounded())                                                                                       <=================
        {
            //Debug.Log("grounded");
            rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, Time.fixedDeltaTime * _stopSpd);
            if (rb.velocity.sqrMagnitude <= 0.005f || _isIdle)
            {
                Stop();
            }
        }*/
        /*        if (rb.velocity.magnitude < _stopSpd)
                {
                    Stop();
                }*/

        ProcessAim();
    }

    private void Update()
    {
        if (_ballTrans.position.y <= -15f)
            StartCoroutine(ResetBall(0.5f));
        CalcInput();
        if (Input.GetMouseButtonDown(0))
        {
            StartShotMouseCords = InputCords;
        }
        if (Input.GetMouseButton(0))
        {
            //ForewardVector = (_camOrbitReference.CamDirFlattened).normalized;
            ForewardVector = Vector3.ProjectOnPlane(camTrans.forward, _ballTrans.up);
            RaycastHit hit;
            Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit);
            //if (_isIdle && hit.collider == _ballCol)
            if (_isIdle)
            {
                //direction of ray on the xz plane
                //Vector3 dir = new Vector3(hit.point.x, _ballTrans.position.y, hit.point.z) - _ballTrans.position;
                //ForewardVector = (dir - _ballTrans.position).normalized;
                //StartCoroutine(ShotDelay());
                _isAiming = true;
                //ProcessAim();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Environment"))
        {
            StartCoroutine(ResetBall(1f));
        }
    }

    #endregion UNITYMETHODS

    #region METHODS

    private Vector3? CastMouseClickRay()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        //ray direction projected onto the xz plane
        //ForewardVector = new Vector3(ray.direction.x, ray.direction.y, ray.direction.z);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return null;
    }

    private bool IsGrounded()
    {
        //Debug.Log(_ballTrans.position + "        " + _ballCol.radius * 10);
        //return Physics.Raycast(_ballTrans.position, Vector3.down, _ballCol.radius * 10);

        return Physics.OverlapSphere(transform.position, _ballCol.radius * 10,
            LayerMask.GetMask("Default")).Length > 0;
    }

    private void ProcessAim()
    {
        /*if (!_isAiming || !_isIdle || HoleManager.GetPause())                                                      <=================
        {
            return;
        }*/

        Vector3? worldPoint = CastMouseClickRay();
        CalcShotPwr();
        if (shotPwr <= 0.2)
        {
            return;
        }
        if (!worldPoint.HasValue)
        {
            return;
        }
        Vector3 ForwardFromBall = (_ballTrans.localToWorldMatrix.GetPosition());
        DrawLine(ForwardFromBall);

        /*if (!Input.GetMouseButton(0) && HoleManager.CanTakeShot())                                                      <=================
        {
            Shoot(worldPoint.Value);
            playerDataSO.SetShotStrength(0);
        }*/
        //canShoot = false;
        /*        if (Input.GetMouseButtonUp(0) && _AimingLineActive)
                {
                    Destroy(_TempLine);
                    _AimingLineActive = false;
                }*/
    }

    private void Shoot(Vector3 worldPoint)
    {
        Vector3 pos = _ballTrans.position;
        _isAiming = false;
        //_lineRenderer.enabled = false;
        /*if (_AimingLineActive)
        {
            Destroy(_TempLine);
            _AimingLineActive = false;
        }*/

        Vector3 horizontalWorldPoint = new Vector3(worldPoint.x, pos.y, worldPoint.z);
        Vector3 ForwardFromBall = ForewardVector.normalized;

        Vector3 direction = (pos - horizontalWorldPoint).normalized;
        //HoleManager.TakeShot();                                                      <=================
        //rb.AddForce(direction * (SliderPower ? playerDataSO.GetShotStrength() : Vector3.Distance(pos, horizontalWorldPoint) * shotPwr), ForceMode.VelocityChange);
        //rb.AddForce(ForwardFromBall * (SliderPower ? playerDataSO.GetShotStrength() : shotPwr * _shotPwrMod), ForceMode.VelocityChange); <=================
        _isIdle = false;
        //playerDataSO.SetIsAiming(false);                                                      <=================
    }

    private void DrawLine(Vector3 worldPoint)
    {
        //playerDataSO.SetIsAiming(true);                                                                           <=================
        //Vector3 horizontalWorldPoint = new Vector3(worldPoint.x, _ballTrans.position.y, worldPoint.z);
        //Vector3 ForwardFromBall = Vector3.ProjectOnPlane((_ballTrans.position + ForewardVector), Vector3.up);
        Vector3 ForwardFromBall = (worldPoint + ForewardVector * shotPwr / 2);
        Vector3[] positions =
        {
            _ballTrans.position,
            ForwardFromBall
    };
        //AimingLine.GetComponent<LineDrawer>().RenderLine(positions);                                                      <=================

        /*        _TempLine = Instantiate(AimingLine, _ballTrans.position, Quaternion.identity);
                _TempLine.GetComponentInChildren<LineRenderer>().SetPositions(positions);*//*
                _lineRenderer.SetPositions(positions);
                _lineRenderer.enabled = true;*/
    }

    private void Stop()
    {
        //rb.velocity = Vector3.zero;                                                      <=================
        rb.angularVelocity = Vector3.zero;
        _ballTrans.rotation = Quaternion.identity;
        _isIdle = true;
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

    private void CalcShotPwr()
    {
        //print("cock");
        shotPwr = (InputCords.x - StartShotMouseCords.x) * 25f;
        shotPwr = Mathf.Clamp(shotPwr, 0, MaxShotPwr);
        //playerDataSO.SetShotStrength(shotPwr);                                                      <=================
    }

    public IEnumerator ResetBall(float t)
    {
        yield return new WaitForSeconds(t);
        //HoleManager.ResetToHole();                                                      <=================
    }

    #endregion METHODS
}