using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float health = 1;

    public float maxHealth = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Damage(float amt, Vector3 direction)
    {
        health -= amt;
        health = Mathf.Max(0, health);
    }

    public void Heal(float amt)
    {
        health += amt;
        health = Mathf.Min(maxHealth, health);
    }
}
