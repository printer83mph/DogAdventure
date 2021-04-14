using System;
using System.Collections;
using Player.Controlling;
using ScriptableObjects;
using ScriptableObjects.Audio;
using ScriptableObjects.Audio.Events;
using ScriptableObjects.World;
using Stims;
using Stims.Receivers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using World;

namespace Player.Inventory
{
    public class FistsController : MonoBehaviour
    {

        [Serializable]
        public class SwingConfig
        {
            [SerializeField] private string animTrigger = null;
            [SerializeField] private Vector3 hitDirection;

            public string AnimTrigger => animTrigger;
            public Vector3 HitDirection => hitDirection;
        }

        [Header("References")]
        [SerializeField] private CameraMovement cameraMovement = null;
        [SerializeField] private GameObject fistsObject = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private PlayerAudioChannel playerAudioChannel = null;
        [SerializeField] private AudioEvent missAudioEvent = null;
        
        [Header("Swing Data")]
        [SerializeField] private SwingConfig swingUp = null;
        [SerializeField] private SwingConfig swingRight = null;
        [SerializeField] private SwingConfig swingDown = null;
        [SerializeField] private SwingConfig swingLeft = null;
        [SerializeField] private SwingConfig swingStraight = null;

        [Header("Impact")]
        [SerializeField] private float swingDamage = 15;
        [SerializeField] private float swingForce = 25;
        [SerializeField] private float swingDistance = 1.4f;
        
        [Header("Swing Control")]
        [SerializeField] private float swingLength = .3f;
        [SerializeField] private float swingDelay = .1f;
        [SerializeField] private float directionVelocityScale = .1f;
        [SerializeField] private float directionAccumulationPeriod = .1f;
        [SerializeField] private float velThreshold = .6f;

        [Header("Hitscan")]
        [SerializeField] private LayerMask layerMask = default;
        [SerializeField] private SurfaceMaterial defaultSurfaceMaterial = null;

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
                Time.deltaTime / directionAccumulationPeriod *
                (1 + cameraMovement.DeltaAim.sqrMagnitude / Time.deltaTime * directionVelocityScale));
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
            
            Debug.Log("Running config " + config.AnimTrigger);
            
            animator.SetTrigger(config.AnimTrigger);

            Ray ray = new Ray(cameraMovement.Orientation.position, cameraMovement.Orientation.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, swingDistance, layerMask))
            {
                Debug.Log("Hit something");
                PunchDamageStim hitStim = new PunchDamageStim(ray, hit, cameraMovement.Orientation.TransformDirection(config.HitDirection), swingDamage, swingForce);
                
                // we hit something?
                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForceAtPosition(hitStim.Force(), hit.point, ForceMode.Impulse);
                }

                // stim receiver
                StimReceiver receiver = hit.transform.GetComponent<StimReceiver>();
                if (receiver)
                {
                    receiver.Stim(hitStim);
                }

                // do world fx
                var material = defaultSurfaceMaterial;
                WorldProperties properties = hit.transform.GetComponent<WorldProperties>();
                if (properties)
                {
                    var propMaterial = properties.SurfaceMaterial;
                    if (propMaterial) material = propMaterial;
                }

                SurfaceMaterial.InstantiateEffects(material, HitType.Fists, hitStim.Point(),
                    Quaternion.FromToRotation(Vector3.forward, hitStim.Normal()),
                    fallback: defaultSurfaceMaterial);

                return;
            }
            
            // if we didn't hit anything:
            playerAudioChannel.PlayEvent(missAudioEvent);
        }
    }
}