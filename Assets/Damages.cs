using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Damages : MonoBehaviour
{
    [SerializeField]
    List<ParticleSystem> indicators;
    [SerializeField]
    float distance;
    void Start()
    {
        
        for(int i = 0; i < 4; i++)
        {
            indicators[i].transform.localPosition = new Vector2(distance * (i % 2) * (i > 1 ? 1 : -1), distance * ((i + 1) % 2) * (i > 1 ? 1 : -1));
        }
        GetComponentsInChildren<ParticleSystem>().ToList().ForEach(ps => orig_sizes.Add(ps, ps.transform.localScale));
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 4; i++)
        {
            SetPsFloat(indicators[i], damages[i]);
            if(damages[i] <= 0f && indicators[i].isEmitting) {
                indicators[i].Stop();
            }
            if (damages[i] > 0f && !indicators[i].isEmitting)
            {
                indicators[i].Play();
            }
        }
    }
    Dictionary<KeyCode, int> map = new Dictionary<KeyCode, int>() { { KeyCode.S, 0 }, { KeyCode.A, 1 }, { KeyCode.W, 2 }, { KeyCode.D, 3 } };
    public void ChangeDamage(KeyCode kc, float value)
    {
        damages[map[kc]] = Mathf.Clamp(damages[map[kc]] + value, 0f, 1f);
    }
    

    public void ChangeDamage(int index, float value)
    {
        damages[index] = Mathf.Clamp(damages[index] + value, 0f, 1f);
    }
    List<float> damages = new List<float>() { 0f, 0f, 0f, 0f };
    [SerializeField]
    float min_emmision, max_emmision, min_multiplier, max_multiplier, min_lifetime, max_lifetime, min_scale, max_scale;
    Dictionary<ParticleSystem, Vector3> orig_sizes = new Dictionary<ParticleSystem, Vector3>();
    void SetPsFloat(ParticleSystem main_ps, float value)
    {
        main_ps.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(ps =>
        {
            ps.emissionRate = Mathf.Lerp(min_emmision, max_emmision, value);
            ParticleSystem.VelocityOverLifetimeModule volm = ps.velocityOverLifetime;
            volm.speedModifierMultiplier = Mathf.Lerp(min_multiplier, max_multiplier, value);
            ParticleSystem.MainModule mm = ps.main;
            mm.startLifetimeMultiplier = Mathf.Lerp(min_lifetime, max_lifetime, value);
            ParticleSystem.SizeOverLifetimeModule solm = ps.sizeOverLifetime;
            solm.sizeMultiplier = Mathf.Lerp(min_scale, max_scale, value);
        });
        
        
    }
}
