using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysicsTick : MonoBehaviour
{
    public bool useRefreshRate = true;
    public float frameRate = 60f;
    
    // Start is called before the first frame update
    void Awake()
    {
        Time.fixedDeltaTime = 1.0f / (useRefreshRate ? Screen.currentResolution.refreshRate : frameRate);
    }
}
