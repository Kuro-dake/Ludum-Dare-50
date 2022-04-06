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
        if (!gaia_attack_done)
        {
            text += "\n<i><size=15>[  spacebar  ]</size></i>";

        }
        int current_length = 0;
        bool open_tag = false;
        while (current_length < text.Length)
        {
            current_length++;
            if (Input.GetKey(KeyCode.Space))
            {
                current_length = text.Length;
                spaced = true;
            }
            text_object.text = text.Substring(0, current_length);
            if (text[current_length - 1] == '<')
            {
                open_tag = true;
            }
            if (text[current_length - 1] == '>')
            {
                open_tag = false;
            }
            if (!open_tag)
            {
                yield return new WaitForSeconds(letter_delay);
            }
        }
        if (gaia_attack_done)
        {
            yield break;
        }
        yield return Common.WaitForKeyDown(KeyCode.Space);
        yield return Make.The(text_object).In(spaced ? .1f : .5f).CGAlphaTo(0f).Execute();
        writing = false;
    }
    [SerializeField]
    float fade_delay, letter_delay = .05f, earth_radius = .51f, spend_speed;
    [SerializeField]
    Damages damages;
    [SerializeField]
    Health health, energy_indicator;
    int phase = -1;
    int current_text = 0;
    static bool repeat = false;
    Sequence s;
    void Start()
    {
        if (repeat)
        {
            phase = -1;
            current_text = 5;
        }

        text_object.text = "";
        game_texts = game_texts_raw.Split(new string[] { "--" }, System.StringSplitOptions.RemoveEmptyEntries);
        s = Sequence.New();
        s += 1f;
        s += () => gaia.Appear();
        s += 1f;

        for (int i = current_text; i < 3; i++)
        {
            s += AdvanceText;
            s += new WaitAction(() => writing);
            s += .5f;
        }
        if (current_text < 4)
        {
            s += (new SimpleLoopableAction(AdvanceText) & (new WaitForSecondsAction(2f) + GenerateMonkey));

            s += new WaitAction(() => writing);

        }
        if (current_text < 5)
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
        if (phase < 2)
        {

            s += AdvanceGame();

            s += AdvanceText; // instructions
            s += new WaitAction(() => writing);
            s += AdvanceText; // instructions
            s += new WaitAction(() => writing);
            s += level_duration;
        }

        s += AdvanceGame();// wood
        s += level_duration;
        s += AdvanceGame();// agro
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += level_duration;

        s += AdvanceGame();// smoke
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += level_duration;

        s += AdvanceGame();// oil
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += level_duration;

        s += AdvanceGame();// war
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += AdvanceText;
        s += new WaitAction(() => writing);

        s += level_duration;

        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += AdvanceGame();// atom
        s += level_duration;


        s += AtomWarStart;
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += AngryMother;
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += AdvanceText;
        s += new WaitAction(() => writing);
        s += GaiaAttack;

        s.Run();
        flash = 0f;
        StartCoroutine(SpawnMonkeys());

    }
    void AngryMother()
    {
        for (int i = 0; i < 4; i++)
        {
            damages.ChangeDamage(i, 1f);
        }
    }
    void AtomWarStart()
    {
        for (int i = 0; i < 4; i++)
                {
                    damages.ChangeDamage(i, -1f);
                }
        atom_war = true;

    }
    float level_duration = 20f;
    IEnumerator SpawnMonkeys()
    {
        while (!gaia_attacked)
        {
            
            yield return new WaitForSeconds(Random.Range(.5f, 1.5f) * (atom_war ? .1f : 1f));
            if (gaia_attacked)
            {
                yield break;
            }
            if (monkeys.Count < (phase + 1) * 5)
            {
                GenerateMonkey();
            }
            yield return null;
        }
    }
    
    void AdvanceText()
    {
        string text = game_texts[current_text++];
        this.text = text;


    }

    [SerializeField]
    List<GameObject> env_prefabs;
    [SerializeField]
    List<FloatRange> disaster_delays;
    Sequence AdvanceGame()
    {

        return Sequence.New() + (new SimpleLoopableAction(() =>
        {
            if (phase <= 5)
            {
                phase++;
                for (int i = 0; i < 4; i++)
                {
                    damages.ChangeDamage(i, -1f);
                }
            }
            else
            {

            }
            AdvanceText();
        }) + new WaitForSecondsAction(1f) + (() => GenerateRandomDisaster(phase)) + new WaitAction(() => writing));
    }

    bool atom_war = false, gaia_attacked = false, gaia_attack_done = false, dead = false;
    float delay = 0f;
    // Update is called once per frame
    void GaiaAttack() {
        atom_war = true;
        gaia_attacked = true;
        gaia.Attack(KillMonkeys, ResetMonkeyStomp).Run();
        for (int i = 0; i < 4; i++)
        {
            damages.ChangeDamage(i, -2f);
        }
    }
    int monkey_stomp = 0;
    void ResetMonkeyStomp() { monkey_stomp = 0;gaia_attack_done = true;

        Make.The(gameObject).ThenWait(5f).then.MakeHappen(() => AdvanceText()).Happen();
    }
    void KillMonkeys()
    {
        int until = monkeys.Count;
        switch (monkey_stomp)
        {
            case 0:
                until = monkeys.Count / 3;
                break;
            case 1:
                until = monkeys.Count / 2;
                break;
            

        }
        PlaySound(7);
        monkey_stomp++;

        for(int i = 0; i < until; i++)
        {
            monkeys[i].Die();
        }

    }
    AudioSource heal_source;
    public void PlaySound(int prefab_number)
    {
        float volume = .025f;
        switch (prefab_number)
        {
            case 0:
                sound.PlayResource("fire", volume,(.96f, 1.05f));
                break;
            case 1:
                sound.PlayResource("boom", volume, (.3f, .35f));
                break;
            case 2:
                sound.PlayResource("fire", volume, (1.96f, 2.05f));
                break;
            case 3:
                sound.PlayResource("fire", volume, (1.96f, 2.05f));
                break;
            case 4:
                sound.PlayResource("fire", volume, (1.96f, 2.05f));
                break;
            case 5:
                sound.PlayResource("boom", volume, (.96f, 1.05f));
                break;
            case 6:
                sound.PlayResource("boom", volume * 1.75f, (.5f, .6f));
                break;
            case 7:
                sound.PlayResource("boom", volume * 6f, (.5f, .6f));
                break;
        }
    }
    void PlayHeal()
    {
        heal_source = sound.PlayResource("heal", .05f, (.95f, 1.05f));
        playing_heal = true;
    }
    void StopHeal()
    {
        if (heal_source != null && heal_source.isPlaying)
        {
            sound.FadeOutSource(heal_source, 2f);
        }
        playing_heal = false;
    }
    bool playing_heal = false;
    void Update()
    {

        if (phase >= 0)
        {
            delay -= Time.deltaTime;
            if (delay < 0f)
            {

                GenerateRandomDisaster();
            }
        }

        if (flash != 0f)
        {
            flash = Mathf.MoveTowards(flash, 0f, Time.deltaTime * flash_fade_speed);
        }

        if (energy <= 0f)
        {
            spent = true;
        }
        if (energy >= 1f)
        {
            spent = false;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            //GaiaAttack();
        }
        bool any_pressed = false;
        if (phase >= 0 && !atom_war && !writing && !dead)
        {

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
            if(any_pressed && !playing_heal)
            {
                PlayHeal();
            }
        }
        else
        {
            controls.ForEach(kc =>
            {
                    damages.UnplayHeal(kc);
                
            });
        }
        if (!any_pressed)
        {
            StopHeal();
        }
        if (phase >= 0)
        {
        if (!any_pressed || spent)
        {
            energy += Time.deltaTime * recharge_speeds[phase];
        }
            energy_indicator.health = atom_war || dead ? 0f : energy;
            health.health = atom_war || dead ? 0f : 1f - damages.sum / hp;

        }
        if(1f - damages.sum / hp < 0f && phase >= 0 && !atom_war)
        {
            repeat = true;
            dead = true;
            text = "<size=100>What the hell monkeys! \n you killed mother!</size>";
            for(int i = 0; i < 4; i++)
            {
                damages.ChangeDamage(i, -1f);
            }
            gaia.Die();
            s.Stop();
            Make.The(gameObject).ThenWait(4f).then.MakeHappen(
            () => UnityEngine.SceneManagement.SceneManager.LoadScene(0)

                ).Happen();
        }
        //if (!writing)
        {
            current_anim_blend = Mathf.MoveTowards(current_anim_blend, gaia_attack_done ? 0f : (atom_war ? 1f : (1f / 8) * phase), Time.deltaTime * (gaia_attack_done ? 10f : .5f));
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
    int last_disaster_rot;
    GameObject GenerateRandomDisaster(int force = -1)
    {
        if (gaia_attacked)
        {
            return null;
        }
        delay = disaster_delays[phase];
        int prefab_num = 0;
        if (atom_war)
        {
            prefab_num = 6;
        }
        else if (force == -1)
        {
            if (phase != 6)
            {
                prefab_num = Random.Range(0, 3) == 1 ? phase : Random.Range(0, phase + 1);
            }
            else
            {
                prefab_num = Random.Range(0, phase + 1);
            }
        }
        else
        {
            prefab_num = force;
        }

        GameObject inst = Instantiate(env_prefabs[prefab_num]);



        Transform parent = new GameObject("d_parent").transform;
        inst.transform.SetParent(parent);
        inst.transform.localPosition = Vector3.up * earth_radius;
        parent.SetParent(surface_parent);
        parent.localPosition = Vector3.zero;

        int rot = force != -1 ? 4 : Random.Range(2, 6);
        if (force == -1)
        {
            while (rot == last_disaster_rot)
            {
                rot = Random.Range(2, 6);
            }
        }
        last_disaster_rot = rot;
        ParticleSystem.MainModule mm = inst.GetComponent<ParticleSystem>().main;
        mm.customSimulationSpace = surface_parent;

        parent.localRotation = Quaternion.Euler(Vector3.forward * (rot * -90f + -20f + earth_counter_rotation));
        if (!writing)
        {
            flash = 1f;
            damages.ChangeDamage(rot - 2, damage_dealt);

        }
        PlaySound(prefab_num);
        if (prefab_num > 4)
        {
            /*Debug.Log("ip " + inst.transform.position);
            Debug.Log("m0 " + monkeys[0].transform.position);
            Debug.Log("dist " + Vector3.Distance(monkeys[0].transform.position, inst.transform.position));*/
            List<Monkey> td = monkeys.FindAll(m => Vector3.Distance(m.GetComponentInChildren<SpriteRenderer>().transform.position, inst.transform.position) < (prefab_num == 5 ? war_radius : atom_radius));
            td.ForEach(m => m.Die());

        }

        return inst;
    }
    [SerializeField]
    float war_radius = .5f, atom_radius = 1f;
    float damage_dealt => damages_dealt[phase];
    AudioManager sound => GetComponent<AudioManager>();
    [SerializeField]
    List<float> damages_dealt, recharge_speeds;
    float earth_counter_rotation => -surface_parent.localRotation.eulerAngles.z - surface_parent.parent.localRotation.eulerAngles.z;
    void GenerateMonkey()
    {
        Monkey m = Instantiate(monkey_prefab, surface_parent);
        m.GetComponentInChildren<SpriteRenderer>().transform.localScale *= Random.Range(.9f, 1.1f);
        monkeys.Add(m);
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
