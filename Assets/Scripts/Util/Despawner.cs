using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawner : MonoBehaviour
{
    public float delay = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, delay);
    }

}
