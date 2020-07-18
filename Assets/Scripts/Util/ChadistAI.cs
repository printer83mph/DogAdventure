using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChadistAI : MonoBehaviour
{
    
    [Header("Config")]
    public float alertLength = 15;
    public int maxSecurityEngaging = 2;

    [Header("For enemy scripts")]
    public Vector3 lastKnownPos;
    public int alertStatus = 0;

    private float _lastSpot;

    void Update() {
        // if it's been long enough then forget abt the player
        if (Time.time - _lastSpot > alertLength) {
            alertStatus = 0;
        }
    }

    public void SpotPlayer(Vector3 pos) {
        lastKnownPos = pos;
        alertStatus = 1;
        _lastSpot = Time.time;
    }

}
