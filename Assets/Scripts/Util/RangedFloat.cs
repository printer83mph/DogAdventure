using System;
using Random = UnityEngine.Random;

namespace Util
{
    [Serializable]
    public class RangedFloat
    {
        
        public float min;
        public float max;
        
        public RangedFloat(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float GetRandom() => Random.Range(min, max);
        
    }
}