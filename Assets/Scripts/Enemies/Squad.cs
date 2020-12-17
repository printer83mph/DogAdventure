using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AlertStatus
{
    None,
    Wary,
    Searching,
    FullAlert
}

public class Squad
{

    public static readonly AlertStatus[] StatusOrder = new[]
        {AlertStatus.None, AlertStatus.Wary, AlertStatus.Searching, AlertStatus.FullAlert};
    
    public List<EnemyBehaviour> members;
    public Vector3 lastKnownPos;

    private AlertStatus _status;
    private int _maxMembers;
    private float _lastAlert;

    public int MaxMembers => _maxMembers;

    public void SetMaxMembers(int maxMembers, bool disbandExtra = true)
    {
        _maxMembers = maxMembers;
        if (!disbandExtra) return;
        while (members.Count > _maxMembers)
        {
            // LIFO
            EnemyBehaviour behaviour = members[0];
            behaviour.ClearSquad();
            members.RemoveAt(0);
        }
    }

    public AlertStatus AlertStatus => _status;

    public void Alert(Vector3 pos)
    {
        if (_status == AlertStatus.Wary || _status == AlertStatus.None)
        {
            lastKnownPos = pos;
            SetAlertStatus(AlertStatus.FullAlert);
        }
    }

    public void SetAlertStatus(AlertStatus status)
    {
        _status = status;
        foreach (EnemyBehaviour behaviour in members)
        {
            // update behaviour stuff when alert status changed
        }
    }

    public void AddMember(EnemyBehaviour behaviour)
    {
        members.Add(behaviour);
        behaviour.SetSquad(this);
        // todo: maybe run something squad side.. update member priorities or something
    }
}
