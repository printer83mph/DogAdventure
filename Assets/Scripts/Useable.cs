using UnityEngine;

public class Useable : MonoBehaviour
{
    public delegate void OnUseDelegate(PlayerInventory inventory);

    public OnUseDelegate onUseDelegate;

    [SerializeField]
    private Renderer[] _renderers;
    public float useDelay = .5f;
    public bool highlighted;
    
    private bool _highlighted;
    private float _lastUse;

    private void Start()
    {
        onUseDelegate += OnUse;
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
        _lastUse = Time.time;
    }

    void Highlight()
    {
        foreach (Renderer renderer in _renderers)
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
        foreach (Renderer renderer in _renderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.SetFloat("glowStrength", 0f);
            }
        }

        _highlighted = false;
    }

    public void Use(PlayerInventory inventory)
    {
        if (Time.time - _lastUse > useDelay) onUseDelegate(inventory);
    }
}
