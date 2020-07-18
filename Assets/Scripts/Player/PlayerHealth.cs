using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{

    public delegate void OnDeathDelegate();
    public OnDeathDelegate onDeathDelegate;

    [HideInInspector]
    public PlayerController _controller;
    public float health = 1;

    public float maxHealth = 1;

    public bool Dead => (health == 0);

    void Awake() {
        _controller = GetComponent<PlayerController>();
        onDeathDelegate += OnDeath;
    }

    public void Damage(float amt, Vector3 direction)
    {
        if (Dead) return;
        health -= amt;
        health = Mathf.Max(0, health);
        if (health == 0) onDeathDelegate();
    }

    public void Heal(float amt)
    {
        if (Dead) return;
        health += amt;
        health = Mathf.Min(maxHealth, health);
    }

    private void OnDeath() {
        Debug.Log("Bro.. you died");
    }

    public void Die() {
        if (Dead) return;
        onDeathDelegate();
    }
}
