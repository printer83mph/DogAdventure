using UnityEngine;

namespace Stims
{
    public interface IStimDamage
    {
        float Damage();
        void SetDamage(float damage);
        DamageType DamageType();
    }
    public interface IStimSource
    {
        StimSource Source();
    }
    public interface IStimPoint
    {
        Vector3 Point();
    }
    public interface IStimNormal
    {
        Vector3 Normal();
    }
    public interface IStimDirection
    {
        Vector3 Direction();
    }
    public interface IStimOrigin
    {
        Vector3 Origin();
    }
    public interface IStimForce : IStimDirection
    {
        Vector3 Force();
    }
    public interface IStimCollision
    {
        Collision Collision();
    }
    public interface IStimPointForce : IStimForce, IStimPoint {}
    public interface IStimRaycast : IStimDirection, IStimOrigin, IStimPoint, IStimNormal
    {
        RaycastHit Hit();
    }
}