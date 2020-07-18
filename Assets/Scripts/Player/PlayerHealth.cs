using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float health = 1;

    public float maxHealth = 1;

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
