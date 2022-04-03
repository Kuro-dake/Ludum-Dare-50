using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleSystemEndDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    ParticleSystem ps;
    // Update is called once per frame
    void Update()
    {
        if(!ps.isEmitting && ps.particleCount == 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
