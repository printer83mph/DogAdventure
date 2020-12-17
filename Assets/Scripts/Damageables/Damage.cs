using UnityEngine;

public abstract class Damage
{
    public enum DamageType
    {
        Slice,
        Bullet,
        BluntForce
    }
    
    public readonly float damage;
    public readonly DamageType damageType;

    protected Damage(float damage, DamageType damageType)
    {
        this.damage = damage;
        this.damageType = damageType;
    }
    
    public abstract class SourcedDamage : Damage
    {
        public readonly DamageSource source;

        protected SourcedDamage(float damage, DamageType damageType, DamageSource source) : base(damage, damageType)
        {
            this.source = source;
        }
    }

    public class BluntForceDamage : SourcedDamage
    {
        public readonly Ray force;

        public BluntForceDamage(float damage, Rigidbody rb, Ray force) : base(damage, DamageType.BluntForce,
            new RigidbodyDamageSource(rb))
        {
            this.force = force;
        }
    }

    public class BulletDamage : SourcedDamage
    {
        public readonly Vector3 direction;
    
        public BulletDamage(float damage, DamageSource source, Vector3 dir) : base(damage, DamageType.Bullet, source)
        {
            direction = dir;
        }
    }
    
    public class PlayerBulletDamage : BulletDamage
    {
        public readonly RaycastHit hit;
    
        public PlayerBulletDamage(float damage, Vector3 dir, RaycastHit hit) : base(damage, GenericDamageSources.Player, dir)
        {
            this.hit = hit;
        }
    }

    public class PlayerKatanaDamage : SourcedDamage
    {
        public PlayerKatanaDamage(float damage) : base(damage, DamageType.Slice, GenericDamageSources.Player)
        {
            // todo: slice info
        }
    }

}