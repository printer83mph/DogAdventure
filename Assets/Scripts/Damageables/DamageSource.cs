using System;
using UnityEngine;

public interface DamageSource
{
    String GetDisplayName();
    Faction GetFaction();
}

public class RigidbodyDamageSource : DamageSource
{
    public Rigidbody rigidbody;
    public string GetDisplayName() => "something moving";
    public Faction GetFaction() => Faction.None;

    private Faction _faction;

    public RigidbodyDamageSource(Rigidbody rb, Faction faction = Faction.None)
    {
        rigidbody = rb;
        this._faction = faction;
    }
}

public class PlayerDamageSource : DamageSource
{
    public Faction GetFaction() => Faction.Ninja;
    public string GetDisplayName() => "the Player";
}

public class EnemyDamageSource : DamageSource
{
    private readonly EnemyBehaviour _enemy;
    private readonly string _name;
    
    public Faction GetFaction() => Faction.Chadist;
    public string GetDisplayName() => _name;

    public EnemyDamageSource(EnemyBehaviour enemy, string name)
    {
        _enemy = enemy;
        _name = name;
    }
}

public static class GenericDamageSources
{
    public static readonly PlayerDamageSource Player = new PlayerDamageSource();
}
