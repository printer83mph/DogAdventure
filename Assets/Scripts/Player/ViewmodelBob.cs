using UnityEngine;

public class ViewmodelBob : MonoBehaviour
{

    public float bobHeight = .005f;
    public float bobSpeed = 3f;
    public float shiftWidth = .003f;
    public float transitionLambda = 10f;
    public float sway = 0.0002f;
    public float swayDelta = 8f;

    [HideInInspector]
    public PlayerController controller;

    private float _halfBobHeight;
    private float _halfShiftWidth;

    private float _distanceMoved;
    private float _speedJumpLerp;
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
        
    }

    public void Bob(Vector3 vel, bool midAir, float dRotX, float dRotY)
    {
        float speedMagnitude = vel.magnitude;

        // bob logic
        float desiredScale = midAir ? 0 : Mathf.Min(speedMagnitude , controller.speed);
        _speedJumpLerp = PrintUtil.Damp(_speedJumpLerp, desiredScale, transitionLambda, Time.deltaTime);
        
        _distanceMoved += speedMagnitude * Time.deltaTime;

        float bounce = (Mathf.Abs(Mathf.Sin(_distanceMoved * bobSpeed)) * bobHeight - _halfBobHeight) * _speedJumpLerp;
        float shift = (Mathf.Cos(_distanceMoved * bobSpeed) * shiftWidth - _halfShiftWidth) * _speedJumpLerp;

        _rotXShift = PrintUtil.Damp(_rotXShift, - dRotY * sway / Time.deltaTime, swayDelta, Time.deltaTime);
        _rotYShift = PrintUtil.Damp(_rotYShift, dRotX * sway / Time.deltaTime, swayDelta, Time.deltaTime);
        
        transform.localPosition = new Vector3(shift + _rotXShift, bounce + _rotYShift, 0);
    }
}
