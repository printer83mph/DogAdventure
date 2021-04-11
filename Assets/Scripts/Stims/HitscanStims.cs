using UnityEngine;

namespace Stims
{
    // hitscan stim
    public class HitscanStim : Stim.Sourced, IStimRaycast, IStimPointForce
    {

        private readonly RaycastHit _hit;
        private readonly Ray _ray;
        private readonly float _force;

        protected HitscanStim(Ray ray, RaycastHit hit, float force, StimSource source) : base(source)
        {
            _hit = hit;
            _ray = ray;
            _force = force;
        }

        public RaycastHit Hit() => _hit;
        public Vector3 Origin() => _ray.origin;
        public Vector3 Direction() => _ray.direction;
        public Vector3 Point() => _hit.point;
        public Vector3 Normal() => _hit.normal;
        public Vector3 Force() => _ray.direction.normalized * _force;
    }
    
    public class HitscanDamageStim : HitscanStim, IStimDamage
    {
            
        private readonly float _damage;
        private readonly DamageType _damageType;
            
        public HitscanDamageStim(float damage, DamageType damageType, Ray ray, RaycastHit hit, float force, StimSource source) : base(ray, hit, force, source)
        {
            _damage = damage;
            _damageType = damageType;
        }

        public float Damage() => _damage;

        public DamageType DamageType() => _damageType;
    }
}