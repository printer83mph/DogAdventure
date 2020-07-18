using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{

    [HideInInspector]
    public PlayerController _controller;
    public float health = 1;

    public float maxHealth = 1;

    void Awake() {
        _controller = GetComponent<PlayerController>();
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
