using UnityEngine;

namespace Stims
{
    // blunt force stim
    public class SourcedForceStim : Stim.Sourced, IStimForce
    {
        private readonly Vector3 _impulse;

        public SourcedForceStim(Vector3 impulse, StimSource source) : base(source)
        {
            _impulse = impulse;
        }

        public Vector3 Force() => _impulse;
        public Vector3 Direction() => _impulse.normalized;
    }

    public class SourcedPointForceStim : SourcedForceStim, IStimPointForce
    {

        private readonly Vector3 _contactPoint;
            
        public SourcedPointForceStim(Vector3 impulse, StimSource source, Vector3 contactPoint) : base(impulse, source)
        {
            _contactPoint = contactPoint;
        }

        public Vector3 Point() => _contactPoint;
    }

    public class CollisionStim : SourcedPointForceStim, IStimCollision
    {

        private readonly Collision _collision;
        
        public CollisionStim(Collision col) : base(col.impulse, new StimSource.Physics(col.collider), col.GetContact(0).point)
        {
            _collision = col;
        }

        public Collision Collision() => _collision;
    }

    public class CollisionDamageStim : CollisionStim, IStimDamage
    {
        private readonly float _damage;
        
        public CollisionDamageStim(Collision col, float damage) : base(col)
        {
            _damage = damage;
        }

        public float Damage() => _damage;

        public DamageType DamageType() => Stims.DamageType.Collision;
    }
}