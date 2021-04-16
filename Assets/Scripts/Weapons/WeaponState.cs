using System;

namespace Weapons
{
    
    [Serializable]
    public abstract class WeaponState { }

    [Serializable]
    public class GunWeaponState : WeaponState
    {
        public GunWeaponState(int startingBullets = 0)
        {
            bullets = startingBullets;
        }
        
        public int bullets = 0;
    }

}