using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraKickController : MonoBehaviour
{
    
    public float bounceVelLambda = 30f;
    public float bounceLambda = 25f;
    
    public float kickVelLambda = 10f;
    public float kickLambda = 10f;
    
    private Quaternion _cameraKickVel;
    private Quaternion _cameraKickRot;
    
    private Vector3 _cameraBounceVel;
    private Vector3 _cameraBouncePos;

    public Vector3 CameraBouncePos => _cameraBouncePos;

    public Quaternion CameraKickRot => _cameraKickRot;

    void Update()
    {
        // rotation
        _cameraKickRot *= (Quaternion.SlerpUnclamped(Quaternion.identity, _cameraKickVel, Time.deltaTime));
        _cameraKickVel = PrintUtil.Damp(_cameraKickVel, Quaternion.identity, kickVelLambda, Time.deltaTime);
        _cameraKickRot = PrintUtil.Damp(_cameraKickRot, Quaternion.identity, kickLambda, Time.deltaTime);
        
        // position
        _cameraBouncePos += _cameraBounceVel * Time.deltaTime;
        _cameraBounceVel = PrintUtil.Damp(_cameraBounceVel, Vector3.zero, bounceVelLambda, Time.deltaTime);
        _cameraBouncePos = PrintUtil.Damp(_cameraBouncePos, Vector3.zero, bounceLambda, Time.deltaTime);
    }
    
    public void AddVel(Vector3 velocity)
    {
        _cameraBounceVel += velocity;
    }

    public void AddKick(Quaternion kick)
    {
        _cameraKickRot *= kick;
    }

}
