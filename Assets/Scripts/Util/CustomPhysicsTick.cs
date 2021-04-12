using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysicsTick : MonoBehaviour
{
    public bool useRefreshRate = true;
    public float frameRate = 60f;
    public float timeScale = 1f;

    private void Update()
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 1.0f / (useRefreshRate ? Screen.currentResolution.refreshRate : frameRate) * timeScale;
    }
}
