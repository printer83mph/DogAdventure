using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecurityEnemyIKController : MonoBehaviour
{
    public LayerMask IKMask = 1;
    public float footOffset = .01f;
    public float downShiftOffset = -.07f;
    public float downShiftScale = .5f;
    private Animator _animator;
    private NavMeshAgent _agent;

    private void Start() {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        float downShift = 0;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2, IKMask)) {
            downShift = (1 - Vector3.Dot(hit.normal, Vector3.up)) * -downShiftScale;
        }
        _agent.baseOffset = downShift + downShiftOffset;
    }

    private void OnAnimatorIK(int layerIndex) {
        if (!_animator) return;
        
        float leftFootWeight = _animator.GetFloat("IKLeftFootWeight");
        float rightFootWeight = _animator.GetFloat("IKLeftFootWeight");

        DoLeftFootIK(leftFootWeight);
        DoRightFootIK(rightFootWeight);
    }

    private void DoLeftFootIK(float weight) {

        // rotation
        Vector3 forwardVec = _animator.GetIKRotation(AvatarIKGoal.LeftFoot) * Vector3.forward;


        Ray castPos = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + (Vector3.up * .2f), Vector3.down);
        if (Physics.Raycast(castPos, out RaycastHit hit, 2, IKMask)) {
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + Vector3.up * footOffset);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weight);
            _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forwardVec, hit.normal));
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, weight);
        }
    }
    private void DoRightFootIK(float weight) {

        // rotation
        Vector3 forwardVec = _animator.GetIKRotation(AvatarIKGoal.RightFoot) * Vector3.forward;


        Ray castPos = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + (Vector3.up * .2f), Vector3.down);
        if (Physics.Raycast(castPos, out RaycastHit hit, 2, IKMask)) {
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, hit.point + Vector3.up * footOffset);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, weight);
            _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forwardVec, hit.normal));
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, weight);
        }
    }
}
