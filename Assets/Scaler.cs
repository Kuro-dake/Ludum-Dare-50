using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * (1.3f + Mathf.Sin(Time.time * .5f) * .1f);
    }
}
