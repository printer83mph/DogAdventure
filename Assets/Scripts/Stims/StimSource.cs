using System;
using UnityEngine;

namespace Stims
{
    public abstract class StimSource
    {
        public abstract String GetDisplayName();
        public abstract Faction GetFaction();

        public class Physics : StimSource
        {
            public readonly Collider collider;
            private Faction _faction;
            public override string GetDisplayName() => "something moving";
            public override Faction GetFaction() => Faction.None;


            public Physics(Collider col = null, Faction faction = Faction.None)
            {
                collider = col;
                this._faction = faction;
            }
        }

        public class Player : StimSource
        {
            public override Faction GetFaction() => Faction.Ninja;
            public override string GetDisplayName() => "the Player";
        }

        public class Enemy : StimSource
        {
            private readonly EnemyBehaviour _behaviour;
            private readonly string _name;

            public override Faction GetFaction() => Faction.Chadist;
            public override string GetDisplayName() => _name;
            public EnemyBehaviour Behaviour => _behaviour;

            public Enemy(EnemyBehaviour behaviour, string name)
            {
                _behaviour = behaviour;
                _name = name;
            }
        }
        
        public static class Generic
        {
            public static readonly Player Player = new Player();
            public static readonly Physics World = new Physics();
        }
    }
}
