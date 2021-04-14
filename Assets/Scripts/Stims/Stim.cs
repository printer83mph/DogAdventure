using UnityEngine;

namespace Stims
{
    
    public enum DamageType
    {
        Slice,
        Bullet,
        Fists,
        ExplosionForce,
        Collision,
        Mystery
    }
    
    public class Stim
    {
        public Stim() {}

        public class MysteryDamage : Stim, IStimDamage
        {

            private readonly float _damage;

            public MysteryDamage(float damage)
            {
                _damage = damage;
            }

            public float Damage() => _damage;
            public DamageType DamageType() => Stims.DamageType.Mystery;
        }

        // generic sourced stim
        public class Sourced : Stim, IStimSource
        {

            private readonly StimSource _source;
            
            public StimSource Source() => _source;
            
            public Sourced(StimSource source) : base()
            {
                _source = source;
            }
            
        }

        public class Katana : Sourced, IStimPointForce, IStimDamage
        {

            public readonly KatanaDamager damager;
            private readonly float _damage;
            private readonly Vector3 _impulseForce;
            private readonly Vector3 _contactPoint;
            
            public Katana(float damage, Vector3 impulseForce, Vector3 contactPoint, KatanaDamager damager) : base(StimSource.Generic.Player)
            {
                this.damager = damager;
                _damage = damage;
                _impulseForce = impulseForce;
                _contactPoint = contactPoint;
            }

            public Vector3 Direction() => _impulseForce.normalized;

            public Vector3 Force() => _impulseForce;

            public Vector3 Point() => _contactPoint;
            public float Damage()
            {
                throw new System.NotImplementedException();
            }

            public DamageType DamageType()
            {
                throw new System.NotImplementedException();
            }
        }

    }
}