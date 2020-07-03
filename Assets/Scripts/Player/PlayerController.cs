using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    // inspector vars
    [Header("Movement")]
    public float speed = 2f;
    public float sprintMultiplier = 1.3f;
    public float groundControl = 30f;
    public float airControl = 10f;
    public float jumpPower = .4f;
    public float gravity = 10f;
    public float maxSlopeAngle = 45f;

    [Header("Camera Movement")]
    public float cameraVelocityShift = .07f;
    public float cameraBounceVelGravity = 4f;
    public float cameraBounceGravity = 4f;
    public float sensitivity = 50f;
    public bool invertMouseY = false;
    
    // auto-assigned
    private Camera _camera;
    private Rigidbody _rb;
    private CapsuleCollider _collider;
    
    // math stuff
    private Vector3 _initialCameraPos;

    private Vector3 _vel;
    private bool _midAir;
    private Vector3 _cameraVel;
    private Vector3 _cameraBouncePos;
    private Vector2 _cameraXZPos;
    private float _lookY;
    private float _lookX;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        _initialCameraPos = _camera.transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // get desired movement
        Vector3 desiredMovement = InputAxisTransform(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 globalDesiredMovement = PlayerRotation() * desiredMovement;
        float actualSpeed = speed * (Input.GetAxis("Sprint") > 0 ? sprintMultiplier : 1);
        
        Quaternion groundRotation = GetGroundRotation();

        if (CheckGrounded() && Quaternion.Angle(groundRotation, Quaternion.identity) < maxSlopeAngle)
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

            print("Midair at " + Time.time);
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
        // todo: currently this lets you climb anything as long as the ground is flat directly underneath you
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            return Quaternion.FromToRotation(transform.up, hit.normal);
        }

        return Quaternion.identity;
    }

    Quaternion PlayerRotation()
    {
        return Quaternion.Euler(0, _lookY, 0);
    }

    void CameraMovement()
    {
        _lookX += (invertMouseY ? 1 : -1) * Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        _lookX = Mathf.Clamp(_lookX, -90, 90);
        
        _lookY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        
        _camera.transform.localRotation = Quaternion.Euler(_lookX, _lookY, 0);
        
        // move camera ahead a bit if grounded
        if (!_midAir)
            _cameraXZPos = Vector2.Lerp(_cameraXZPos, new Vector2(_vel.x, _vel.z) * cameraVelocityShift, Time.deltaTime * 10f);
        else // center if in air
            _cameraXZPos =
                Vector3.Lerp(_cameraXZPos, Vector2.zero, Time.deltaTime * 10f);

        // do ground hit bounce animation
        _cameraVel = Vector3.MoveTowards(_cameraVel, Vector3.zero, Time.deltaTime * cameraBounceVelGravity);
        _cameraBouncePos += _cameraVel * Time.deltaTime;
        _cameraBouncePos = Vector3.Lerp(_cameraBouncePos, Vector3.zero, Time.deltaTime * cameraBounceGravity);
        
        // todo: probably shouldn't just use the y value
        _camera.transform.localPosition = _initialCameraPos + new Vector3(_cameraXZPos.x, 0, _cameraXZPos.y) + new Vector3(0, _cameraBouncePos.y, 0);

    }

    Vector3 InputAxisTransform(float hor, float vert)
    {
        return new Vector3(hor * (float)Mathf.Sqrt(1 - Mathf.Pow(vert, 2) / 2), 0, vert * (float)Mathf.Sqrt(1 - Mathf.Pow(hor, 2) / 2));
    }

    bool CheckGrounded()
    {
        Vector3 startSpot = new Vector3(_collider.bounds.center.x, _collider.bounds.min.y + _collider.radius + .1f,
            _collider.bounds.center.z);
        if (Physics.SphereCast(startSpot, _collider.radius, Vector3.down, out RaycastHit hit, .2f, ~(1 << 8)))
        {
            // cancel downwards velocity
            if (_midAir)
            {
                _cameraVel = _vel;
                _midAir = false;
            }
            _vel -= Mathf.Min(Vector3.Dot(hit.normal, _vel), 0) * hit.normal;
            return true;
        }

        _midAir = true;
        return false;
    }
}
