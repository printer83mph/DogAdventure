using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{

    // inspector vars
    [Header("Movement")]
    public float speed = 4f;
    public float sprintMultiplier = 1.5f;
    public float groundControl = 40f;
    public float airControl = 16f;
    public float jumpPower = 5f;
    public float gravity = 13f;
    public float maxSlopeAngle = 45f;
    public LayerMask standable = 0;

    [Header("Camera Movement")]
    public Camera cam;
    public bool cameraVelocityShift = true;
    public float cameraVelocityShiftScale = .04f;

    public bool landingBounce = true;
    public float landingBounceScale = 1f;

    public ViewmodelBob viewmodelBob;
    public CameraKickController kickController;
    
    [Header("Mouse Controls")]
    public float sensitivity = 100f;
    public bool invertMouseY = false;
    
    // auto-assigned
    private Rigidbody _rb;
    private CapsuleCollider _collider;
    private PlayerHealth _health;
    [HideInInspector]
    public PlayerInput input;
    
    // math stuff
    private Vector3 _initialCameraPos;

    private Vector3 _vel;
    private Quaternion _groundRotation;
    private Vector3 _groundVelocity;
    private Rigidbody _groundRigidbody;
    private bool _midAir;
    private Vector2 _cameraXZPos;
    private float _lookY;
    private float _lookX;
    private float _dRotY;
    private float _dRotX;

    // input stuff
    private InputAction m_Move;
    private InputAction m_Sprint;
    private InputAction m_Aim;

    public float DRotX => _dRotX;
    public float DRotY => _dRotY;
    public bool MidAir => _midAir;
    public Vector3 Vel => _vel;

    private void Awake()
    { 
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _health = GetComponent<PlayerHealth>();
        
        viewmodelBob.controller = this;

        // setup input stuff
        input = GetComponent<PlayerInput>();
        m_Move = input.actions["Move"];
        m_Sprint = input.actions["Sprint"];
        m_Aim = input.actions["Aim"];
    }

    private void OnEnable() {
        input.actions["Jump"].performed += JumpAction;
        _health.onDeathDelegate += OnDeath;
    }

    private void OnDisable() {
        input.actions["Jump"].performed -= JumpAction;
        _health.onDeathDelegate -= OnDeath;
    }

    void JumpAction(InputAction.CallbackContext ctx) {
        Jump();
    }

    // Start is called before the first frame update
    void Start()
    {
        _initialCameraPos = cam.transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // get desired movement
        Vector2 moveInput = m_Move.ReadValue<Vector2>();
        Vector3 desiredMovement = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 globalDesiredMovement = PlayerRotation() * desiredMovement;
        float actualSpeed = speed * (m_Sprint.ReadValue<float>() > 0 ? sprintMultiplier : 1);
        
        GetGroundData();

        if (CheckGrounded())
        {

            // ground movement
            _vel = Vector3.MoveTowards(_vel, _groundRotation * (globalDesiredMovement * actualSpeed) + _groundVelocity, Time.fixedDeltaTime * groundControl);

        }
        else
        {

            // stop in midair but dont gain vel
            Vector2 xzVel = new Vector2(_vel.x, _vel.z);
            Vector2 desiredXZVel = new Vector2(globalDesiredMovement.x, globalDesiredMovement.z);

            float velDot = Vector2.Dot(xzVel, desiredXZVel);
            // Vector2 newXZVel = xzVel - desiredXZVel * Mathf.Min(velDot * airControl, 0);
            // _vel.x = newXZVel.x;
            // _vel.z = newXZVel.y;

            _vel += globalDesiredMovement * (airControl * Mathf.Max(speed - velDot, 0) * Time.fixedDeltaTime);
            
            _vel.y -= gravity * Time.fixedDeltaTime;
        }
        
        // move based on velocity
        _rb.MovePosition(transform.position + _vel * Time.fixedDeltaTime);
        _rb.velocity = Vector3.zero;

    }

    void Jump() {
        if (_midAir) return;
        if (_groundRigidbody) {
            float massRatio = _groundRigidbody.mass / _rb.mass;
            _groundRigidbody.AddForceAtPosition(new Vector3(0, -jumpPower * _rb.mass, 0), new Vector3(_collider.bounds.center.x, _collider.bounds.min.y, _collider.bounds.center.z), ForceMode.Impulse);
            // _vel.y += jumpPower * massRatio;
        }
        _vel.y += jumpPower + _groundVelocity.y;
    }

    private void OnCollisionEnter(Collision other) {
        // TODO: fix jumping while next to charging enemies
        Debug.Log("We hit something bro...");
        // if (other.collider.gameObject.isStatic)
        // {
            ContactPoint point = other.GetContact(0);
            // _vel -= other.impulse;
            float massRatio = other.rigidbody ? Mathf.Min(other.rigidbody.mass / _rb.mass) : 1;
            Debug.Log(massRatio);
            _vel -= point.normal * (Mathf.Min(Vector3.Dot(_vel - other.relativeVelocity, point.normal), 0) * massRatio);
            if (landingBounce) kickController.AddVel(Vector3.up * ((_vel.y - _groundVelocity.y) * landingBounceScale));
        // }
    }
    
    void LateUpdate()
    {
        CameraMovement();
    }

    void GetGroundData()
    {
        _groundRotation = Quaternion.identity;
        _groundVelocity = Vector3.zero;
        if (Physics.Raycast(new Vector3(transform.position.x, _collider.bounds.min.y, transform.position.z), Vector3.down, out RaycastHit hit, .2f))
        {
            Quaternion angle = Quaternion.FromToRotation(transform.up, hit.normal);
            _groundRigidbody = hit.transform.GetComponent<Rigidbody>();

            // only do shit if its walkable
            if (Quaternion.Angle(angle, Quaternion.identity) < maxSlopeAngle)
            {
                _groundRotation = angle;
                if (_groundRigidbody) _groundVelocity = _groundRigidbody.GetPointVelocity(hit.point);
            }
        }
    }

    Quaternion PlayerRotation()
    {
        return Quaternion.Euler(0, _lookY, 0);
    }

    void CameraMovement()
    {
        Vector2 deltaAim = m_Aim.ReadValue<Vector2>();
        _dRotX = (invertMouseY ? 1 : -1) * deltaAim.y * sensitivity;
        _lookX += _dRotX;
        _lookX = Mathf.Clamp(_lookX, -90, 90);
        
        _dRotY = deltaAim.x * sensitivity;
        _lookY += _dRotY;

        cam.transform.localRotation = Quaternion.Euler(_lookX, _lookY, 0) * kickController.CameraKickRot;

        // move camera ahead a bit if grounded
        Vector2 relativeVel = new Vector2(_vel.x - _groundVelocity.x, _vel.z - _groundVelocity.z);
        _cameraXZPos = !_midAir && cameraVelocityShift
            ? PrintUtil.Damp(_cameraXZPos, relativeVel * cameraVelocityShiftScale, 10f,
                Time.deltaTime)
            : PrintUtil.Damp(_cameraXZPos, Vector2.zero, 10f, Time.deltaTime);

        cam.transform.localPosition = _initialCameraPos + new Vector3(_cameraXZPos.x, 0, _cameraXZPos.y);
        cam.transform.position += kickController.CameraBouncePos;
        
    }

    bool CheckGrounded()
    {
        Vector3 startSpot = new Vector3(_collider.bounds.center.x, _collider.bounds.min.y + _collider.radius + .1f,
            _collider.bounds.center.z);
        if (Physics.SphereCast(startSpot, _collider.radius / 2 + .1f, Vector3.down, out RaycastHit hit, _collider.radius / 2 + .05f, standable))
        {
            // cancel downwards velocity
            if (_midAir)
            {
                if (landingBounce) kickController.AddVel(Vector3.up * ((_vel.y - _groundVelocity.y) * landingBounceScale));
                _midAir = false;
            }
            _vel -= Mathf.Min(Vector3.Dot(hit.normal, _vel), 0) * hit.normal;
            return true;
        }

        _midAir = true;
        return false;
    }

    private void OnDeath() {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        col.height = .2f;
        col.center = col.center + Vector3.up * .4f;

        _rb.useGravity = true;
        _rb.velocity = _vel;

        cam.transform.Rotate(0, 0, -45);

        enabled = false;
    }

}
