using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IEnumRunner;
using IEnumRunner.Transitions;
public class GM : MonoBehaviour
{
    [SerializeField]
    Gaia gaia;
    [SerializeField]
    TMPro.TextMeshProUGUI text_object;
    
    string game_texts_raw => Resources.Load<TextAsset>("texts").text;
    string[] game_texts;
    string text
    {
        set => StartCoroutine(WriteText(value));
    }
    bool writing;
    bool spaced;
    IEnumerator WaitForSecondsOrSpace(float delay)
    {
        float t = delay;
        spaced = false;
        while (t > 0f)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                spaced = true;
                t = 0f;
            }
            t -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator WriteText(string text)
    {
        
        writing = true;
        Make.The(text_object).instantly.CGAlphaTo(1f).Happen();
        
        int current_length = 0;
        
        while(current_length < text.Length)
        {
            current_length++;
            if (Input.GetKey(KeyCode.Space))
            {
                current_length = text.Length;
                spaced = true;
            }
            text_object.text = text.Substring(0, current_length);
            yield return new WaitForSeconds(letter_delay);
        }
        yield return WaitForSecondsOrSpace(fade_delay);
        yield return Make.The(text_object).In(spaced ? .1f : .5f).CGAlphaTo(0f).Execute();
        writing = false;
    }
    [SerializeField]
    float fade_delay, letter_delay = .05f, earth_radius = .51f, spend_speed;
    [SerializeField]
    Damages damages;
    [SerializeField]
    Health health, energy_indicator;
    void Start()
    {
        text_object.text = "";
        game_texts = game_texts_raw.Split(new string[]{"--"},System.StringSplitOptions.RemoveEmptyEntries);
        Sequence s = Sequence.New();
        s += 1f;
        s += () => gaia.Appear();
        s += 1f;
        
        for(int i = current_text; i < 3; i++)
        {
            s += AdvanceText;
            s += new WaitAction(() => writing);
            s += .5f;
        }
        if(current_text < 4)
        {
            s += (new SimpleLoopableAction(AdvanceText) & (new WaitForSecondsAction(2f) + GenerateMonkey) );
        
            s += new WaitAction(() => writing);

        }
        if(current_text < 5)
        {

            float monkey_spawn_delay = .25f;
            s += (new SimpleLoopableAction(AdvanceText) 
                & (new WaitForSecondsAction(2f) + GenerateMonkey 
                + monkey_spawn_delay + GenerateMonkey 
                + monkey_spawn_delay + GenerateMonkey 
                + monkey_spawn_delay + GenerateMonkey));
            
            s += new WaitAction(() => writing);
        }
        

        s += 1f;

        s += AdvanceGame();
        s += new WaitAction(() => writing);
        s += AdvanceText;

        s.Run();
        flash = 0f;
        StartCoroutine(SpawnMonkeys());
        
    }
    IEnumerator SpawnMonkeys()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(.5f, 1.5f));
            if(monkeys.Count < (phase + 1) * 5)
            {
                GenerateMonkey();
            }
            yield return null;
        }
    }
    int current_text = 5;
    void AdvanceText()
    {
        string text = game_texts[current_text++];
        this.text = text;
        
        
    }
    int phase = -1;
    [SerializeField]
    List<GameObject> env_prefabs;
    [SerializeField]
    List<FloatRange> disaster_delays;
    Sequence AdvanceGame()
    {
        
        return Sequence.New() + (new SimpleLoopableAction(()=> {
            if (phase <= 5)
            {
                phase++;
                for(int i = 0; i < 4; i++)
                {
                    damages.ChangeDamage(i, 0f);
                }
            }
            else
            {
                atom_war = true;
                for (int i = 0; i < 4; i++)
                {
                    damages.ChangeDamage(i, 1f);
                }
            }
            AdvanceText(); 
        }) + new WaitForSecondsAction(1f) + (()=>GenerateRandomDisaster(phase)));
    }

    bool atom_war = false;
    float delay = 0f;
    // Update is called once per frame
    void Update()
    {
        if(phase < 0)
        {
            return;
        }
        if(phase >= 0)
        {
            delay -= Time.deltaTime;
            if(delay < 0f)
            {
                
                GenerateRandomDisaster();
            }
        }
        
        if(flash != 0f)
        {
            flash = Mathf.MoveTowards(flash, 0f, Time.deltaTime * flash_fade_speed);
        }
        
        if(energy <= 0f)
        {
            spent = true;
        }
        if(energy >= 1f)
        {
            spent = false;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            AdvanceGame().Run();
        }
        
        bool any_pressed = false;
        controls.ForEach(kc =>
        {
            if (Input.GetKey(kc))
            {
                any_pressed = true;
                if (spent)
                {
                    damages.UnplayHeal(kc);
                    return;
                }
                
                damages.ChangeDamage(kc, -Time.deltaTime);
                energy = Mathf.Clamp(energy - spend_speed * Time.deltaTime, 0f, 1f);
            }
            else
            {
                damages.UnplayHeal(kc);
            }
        });
        
        if (!any_pressed || spent)
        {
            energy += Time.deltaTime * recharge_speeds[phase];
        }
        energy_indicator.health = atom_war ? 0f : energy;
        health.health = atom_war ? 0f : 1f - damages.sum / hp;
        //if (!writing)
        {
            current_anim_blend = Mathf.MoveTowards(current_anim_blend, (1f/6)*phase, Time.deltaTime * .5f);
            gaia.GetComponent<Animator>().SetFloat("Blend", current_anim_blend);
            flash_parent.GetComponent<Animator>().SetFloat("Blend", current_anim_blend);

        }
    }
    float current_anim_blend = 0f;
    List<Monkey> _monkeys = new List<Monkey>();
    List<Monkey> monkeys
    {
        get
        {
            _monkeys.RemoveAll(m => m == null);
            return _monkeys;
        }
    }
    bool spent = false;
    float energy = 1f;
    [SerializeField]
    float hp = 2f;
    List<KeyCode> controls = new List<KeyCode>() { KeyCode.A, KeyCode.W, KeyCode.S, KeyCode.D };

    GameObject GenerateRandomDisaster(int force = -1)
    {
        delay = disaster_delays[phase];
        GameObject inst = Instantiate(env_prefabs[atom_war ? 6 : (force == -1?Random.Range(0, phase + 1) : force)]);
        Transform parent = new GameObject("d_parent").transform;
        inst.transform.SetParent(parent);
        inst.transform.localPosition = Vector3.up * earth_radius;
        parent.SetParent(surface_parent);
        parent.localPosition = Vector3.zero;

        int rot = force != -1 ? 4 : Random.Range(2, 6);

        ParticleSystem.MainModule mm = inst.GetComponent<ParticleSystem>().main;
        mm.customSimulationSpace = surface_parent;

        parent.localRotation = Quaternion.Euler(Vector3.forward * (rot * -90f + -20f + earth_counter_rotation));
        flash = 1f;
        if (!writing)
        {
            damages.ChangeDamage(rot - 2, damage_dealt);

        }
        return inst;
    }
    float damage_dealt => damages_dealt[phase];
    
    [SerializeField]
    List<float> damages_dealt,recharge_speeds;
    float earth_counter_rotation => -surface_parent.localRotation.eulerAngles.z - surface_parent.parent.localRotation.eulerAngles.z;
    void GenerateMonkey()
    {
        monkeys.Add(Instantiate(monkey_prefab, surface_parent));
    }

    [SerializeField]
    Monkey monkey_prefab;
    [SerializeField]
    Transform surface_parent;
    [SerializeField]
    Transform flash_parent;
    [SerializeField]
    Color flash_color, no_flash_color;
    [SerializeField]
    float flash_fade_speed = 2f;
    float _flash = 0f;
    float flash
    {
        get => _flash;
        set
        {
            _flash = value;
            flash_parent.GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(sr => sr.color = Color.Lerp(no_flash_color, flash_color, value));
        }
    }
}
