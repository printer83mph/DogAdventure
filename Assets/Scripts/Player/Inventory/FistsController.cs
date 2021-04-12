using System;
using System.Collections;
using Player.Controlling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.Inventory
{
    public class FistsController : MonoBehaviour
    {

        [Serializable]
        public class SwingConfig
        {
            [SerializeField] private string animTrigger = null;
            [SerializeField] private Transform hitscanStart = null;
            [SerializeField] private Transform hitscanEnd = null;

            public string AnimTrigger => animTrigger;
            public Transform HitscanStart => hitscanStart;
            public Transform HitscanEnd => hitscanEnd;
        }

        [SerializeField] private SwingConfig swingUp;
        [SerializeField] private SwingConfig swingRight;
        [SerializeField] private SwingConfig swingDown;
        [SerializeField] private SwingConfig swingLeft;
        [SerializeField] private SwingConfig swingStraight;

        [SerializeField] private CameraMovement cameraMovement;
        [SerializeField] private GameObject fistsObject;
        [SerializeField] private Animator animator;

        [SerializeField] private float swingLength = .4f;
        [SerializeField] private float swingDelay = .1f;
        [SerializeField] private float directionAccumulationPeriod = .2f;
        [SerializeField] private float velThreshold = 50;

        private Vector2 _lookVelLerp = default;
        private bool _swinging = false;
        private bool _active = false;

        public bool Swinging => _swinging;
        public bool Active => _active;
        
        public void Enable()
        {
            fistsObject.SetActive(true);
            _active = true;
        }

        public void Disable()
        {
            fistsObject.SetActive(false);
            StopCoroutine(SwingCoroutine());
            _active = false;
        }

        private void Update()
        {
            _lookVelLerp = Vector2.MoveTowards(_lookVelLerp, cameraMovement.DeltaAim, 
                Time.deltaTime / directionAccumulationPeriod);
        }

        public void RequestSwing()
        {
            if (_swinging) return;

            StartCoroutine(SwingCoroutine());
        }

        private IEnumerator SwingCoroutine()
        {
            _swinging = true;
            yield return new WaitForSeconds(swingDelay);
            Swing();
            yield return new WaitForSeconds(swingLength);
            _swinging = false;
        }

        private void Swing()
        {
            if (_lookVelLerp.sqrMagnitude < Mathf.Pow(velThreshold, 2))
            {
                Debug.Log("Going straight");
                RunPunch(swingStraight);
                return;
            }
            float angle = Vector2.SignedAngle(new Vector2(-1, -1), _lookVelLerp);
            Debug.Log(angle);
            SwingConfig config = null;
            if (angle > 90)
            {
                config = swingRight;
            }
            else if (angle > 0)
            {
                config = swingDown;
            }
            else if (angle > -90)
            {
                config = swingLeft;
            }
            else
            {
                config = swingUp;
            }
            
            RunPunch(config);
        }

        public void Reset()
        {
            // animator.SetTrigger("Reset");
        }
        
        private void RunPunch(SwingConfig config)
        {
            animator.SetTrigger(config.AnimTrigger);
            // do raycast logic
        }
    }
}