using UnityEngine;

public class ViewmodelBob : MonoBehaviour
{

    public float bobHeight = .015f;
    public float bobSpeed = 5f;
    public float shiftWidth = .04f;
    public float transitionLambda = 5f;
    public float sway = 0.0002f;
    public float swayLambda = 8f;
    public float verticalVelocityInfluence = -.02f;
    public float verticalInfluenceClamp = .1f;

    [HideInInspector]
    public PlayerController controller;

    private float _halfBobHeight;
    private float _halfShiftWidth;

    private float _distanceMoved;
    private float _speedJumpLerp;
    private float _sprintLerp;
    private float _verticalVelocityShift;
    private float _rotXShift;
    private float _rotYShift;

    // Start is called before the first frame update
    void Start()
    {
        _halfBobHeight = bobHeight / 2;
        _halfShiftWidth = shiftWidth / 2;
    }

    // Update is called once per frame
    void Update()
    {
        float speedMagnitude = controller.Vel.magnitude;

        // bob logic
        float desiredScale = controller.MidAir ? 0 : Mathf.Min(speedMagnitude / controller.speed , 1);
        _speedJumpLerp = PrintUtil.Damp(_speedJumpLerp, desiredScale, transitionLambda, Time.deltaTime);
        float desiredSprintScale = controller.MidAir ? 0 : Mathf.Clamp(speedMagnitude / controller.speed - 1, 0, 1);
        _sprintLerp = PrintUtil.Damp(_sprintLerp, desiredSprintScale, transitionLambda, Time.deltaTime);
        
        _distanceMoved += speedMagnitude * Time.deltaTime;

        float bounce = (Mathf.Abs(Mathf.Sin(_distanceMoved * bobSpeed / controller.speed)) * bobHeight - _halfBobHeight) * _speedJumpLerp;
        float shift = (Mathf.Cos(_distanceMoved * bobSpeed / controller.speed) * shiftWidth - _halfShiftWidth) * _sprintLerp;

        _rotXShift = PrintUtil.Damp(_rotXShift, - controller.DRotY * sway / Time.deltaTime, swayLambda, Time.deltaTime);
        _rotYShift = PrintUtil.Damp(_rotYShift, controller.DRotX * sway / Time.deltaTime, swayLambda, Time.deltaTime);

        float desiredVerticalShift = controller.MidAir ? Mathf.Clamp(controller.Vel.y * verticalVelocityInfluence,
            -verticalInfluenceClamp, verticalInfluenceClamp) : 0;
        _verticalVelocityShift = PrintUtil.Damp(_verticalVelocityShift, desiredVerticalShift, transitionLambda, Time.deltaTime);
        
        transform.localPosition = new Vector3(shift + _rotXShift, bounce + _rotYShift + _verticalVelocityShift, 0);
    }

}
