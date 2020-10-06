using System.Collections.Generic;
using UnityEngine;

public class Useable : MonoBehaviour
{
    public delegate void OnUseDelegate(PlayerInventory inventory);

    public OnUseDelegate onUseDelegate;

    public Renderer[] renderers;
    // public float useDelay = .5f;
    public bool highlighted;
    
    private bool _highlighted;
    private float _lastUse;

    public static List<Useable> useables = new List<Useable>();

    private void Awake() {
        onUseDelegate += OnUse;
    }

    private void OnEnable() {
        useables.Add(this);
    }

    private void OnDisable() {
        useables.Remove(this);
    }

    private void Update()
    {
        if (highlighted != _highlighted)
        {
            if (highlighted) Highlight();
            else UnHighlight();
        }
    }

    void OnUse(PlayerInventory inventory)
    {
        // _lastUse = Time.time;
    }

    void Highlight()
    {
        // TODO: refactor this so it's just UI based (no icky material stuff)
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.SetFloat("glowStrength", 1f);
            }
        }

        _highlighted = true;
    }

    void UnHighlight()
    {
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.SetFloat("glowStrength", 0.2f);
            }
        }

        _highlighted = false;
    }

    public void Use(PlayerInventory inventory)
    {
        // if (Time.time - _lastUse > useDelay) 
        onUseDelegate(inventory);
    }
}
