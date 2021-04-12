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
        public virtual Vector3 Origin() => _ray.origin;
        public virtual Vector3 Direction() => _ray.direction;
        public virtual Vector3 Point() => _hit.point;
        public virtual Vector3 Normal() => _hit.normal;
        public virtual Vector3 Force() => _ray.direction.normalized * _force;
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

    public class PunchDamageStim : Stim.Sourced, IStimRaycast, IStimPointForce, IStimDamage
    {

        private readonly Ray _punchCheckRay;
        private readonly RaycastHit _hit;
        private readonly Vector3 _punchDirection;
        private readonly float _damage;
        private readonly float _force;
        
        public PunchDamageStim(Ray punchCheckRay, RaycastHit hit, Vector3 punchDirection, float damage, float force) : base(StimSource.Generic.Player)
        {
            _punchCheckRay = punchCheckRay;
            _hit = hit;
            _punchDirection = punchDirection;
            _damage = damage;
            _force = force;
        }

        public Vector3 Direction() => _punchDirection;
        public Vector3 Force() => _force * _punchDirection;
        public Vector3 Origin() => _punchCheckRay.origin;
        public Vector3 Point() => _hit.point;
        public Vector3 Normal() => _hit.normal;
        public RaycastHit Hit() => _hit;
        public float Damage() => _damage;
        public DamageType DamageType() => Stims.DamageType.Fists;
    }
    
}