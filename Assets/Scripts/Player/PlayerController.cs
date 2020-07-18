using UnityEngine;

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
    public LayerMask standable;

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
    
    // math stuff
    private Vector3 _initialCameraPos;

    private Vector3 _vel;
    private bool _midAir;
    private Vector2 _cameraXZPos;
    private float _lookY;
    private float _lookX;
    private float _dRotY;
    private float _dRotX;

    public float DRotX => _dRotX;
    public float DRotY => _dRotY;
    public bool MidAir => _midAir;
    public Vector3 Vel => _vel;

    private void Awake()
    { 
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _initialCameraPos = cam.transform.localPosition;
        viewmodelBob.controller = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // get desired movement
        Vector3 desiredMovement = InputAxisTransform(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 globalDesiredMovement = PlayerRotation() * desiredMovement;
        float actualSpeed = speed * (Input.GetAxis("Sprint") > 0 ? sprintMultiplier : 1);
        
        Quaternion groundRotation = GetGroundRotation();

        if (CheckGrounded())
        {

            // ground movement
            _vel = Vector3.MoveTowards(_vel, groundRotation * (globalDesiredMovement * actualSpeed), Time.fixedDeltaTime * groundControl);

            // detect jump
            if (Input.GetAxis("Jump") > 0)
            {
                _vel.y = jumpPower;
            }
        }
        else
        {

            // print("Midair at " + Time.time);
            Vector2 newVel = Vector2.MoveTowards(new Vector2(_vel.x, _vel.z),
                new Vector2(globalDesiredMovement.x, globalDesiredMovement.z) * actualSpeed, Time.fixedDeltaTime * airControl);

            _vel.x = newVel.x;
            _vel.z = newVel.y;
            
            _vel.y -= gravity * Time.fixedDeltaTime;
        }
        
        // move based on velocity
        _rb.MovePosition(transform.position + _vel * Time.fixedDeltaTime);
        _rb.velocity = Vector3.zero;

    }
    
    void LateUpdate()
    {
        CameraMovement();
    }

    Quaternion GetGroundRotation()
    {
        if (Physics.Raycast(new Vector3(transform.position.x, _collider.bounds.min.y, transform.position.z), Vector3.down, out RaycastHit hit, .2f))
        {
            Quaternion angle = Quaternion.FromToRotation(transform.up, hit.normal);

            // only return the angle if it's walkable
            if (Quaternion.Angle(angle, Quaternion.identity) < maxSlopeAngle) return angle;
        }

        return Quaternion.identity;
    }

    Quaternion PlayerRotation()
    {
        return Quaternion.Euler(0, _lookY, 0);
    }

    void CameraMovement()
    {
        _dRotX = (invertMouseY ? 1 : -1) * Input.GetAxis("Mouse Y") * sensitivity;
        _lookX += _dRotX;
        _lookX = Mathf.Clamp(_lookX, -90, 90);
        
        _dRotY = Input.GetAxis("Mouse X") * sensitivity;
        _lookY += _dRotY;

        cam.transform.localRotation = Quaternion.Euler(_lookX, _lookY, 0) * kickController.CameraKickRot;

        // move camera ahead a bit if grounded
        _cameraXZPos = !_midAir && cameraVelocityShift
            ? PrintUtil.Damp(_cameraXZPos, new Vector2(_vel.x, _vel.z) * cameraVelocityShiftScale, 10f,
                Time.deltaTime)
            : PrintUtil.Damp(_cameraXZPos, Vector2.zero, 10f, Time.deltaTime);

        // todo: probably shouldn't just use the y value
        // todo: camera bobbing maybe
        cam.transform.localPosition = _initialCameraPos + new Vector3(_cameraXZPos.x, 0, _cameraXZPos.y);
        cam.transform.position += kickController.CameraBouncePos;
        
    }

    Vector3 InputAxisTransform(float hor, float vert)
    {
        return new Vector3(hor * (float)Mathf.Sqrt(1 - Mathf.Pow(vert, 2) / 2), 0, vert * (float)Mathf.Sqrt(1 - Mathf.Pow(hor, 2) / 2));
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
                if (landingBounce) kickController.AddVel(Vector3.up * (_vel.y * landingBounceScale));
                _midAir = false;
            }
            _vel -= Mathf.Min(Vector3.Dot(hit.normal, _vel), 0) * hit.normal;
            return true;
        }

        _midAir = true;
        return false;
    }

    public void AddToViewmodel(Transform newGuy)
    {
        newGuy.parent = viewmodelBob.transform;
    }
}
