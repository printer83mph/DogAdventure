using System;
using UnityEngine;

namespace Weapons.Guns
{
    public abstract class FalloffEffect
    {
        public abstract float Evaluate(float baseNumber, float distance);

        public class LimitedExponential : FalloffEffect
        {

            private readonly float _exponent;
            private readonly float _maxDistance;
            
            public LimitedExponential(float exponent, float maxDistance)
            {
                _exponent = exponent;
                _maxDistance = maxDistance;
            }
            
            public override float Evaluate(float baseNumber, float distance)
            {
                return baseNumber * (1 - Mathf.Pow(Mathf.Clamp(distance / _maxDistance, 0, 1), _exponent));
            }
        }
    }
}