using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IEnumRunner;
using IEnumRunner.Transitions;
public class Gaia : MonoBehaviour
{
    Vector3 startpos, orig_root_pos;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.localPosition;
        orig_root_pos = root_bone.position;
        root_bone.localPosition += Vector3.up * 15f;
    }
    [SerializeField]
    Transform flash_parent;
    public void Appear()
    {
        cloud_ps.Play();

        Make.The(root_bone)
            .instantly.ScaleTo(new Vector3(.4f, 1.5f, .1f))
            .ThenWait(1f)
            .then.In(.15f).MoveTo(orig_root_pos)
            .then.In(.1f).ScaleTo(new Vector3(1.1f, .9f, .1f))
            .then.In(.1f).ScaleTo(Vector3.one)
            .Happen();
    }

    [SerializeField]
    float speed, range;
    float float_time;
    // Update is called once per frame
    bool attacking = false;
    void Update()
    {
        float_time += attacking ? 0f : Time.deltaTime * speed;
        transform.position = flash_parent.position = startpos + Vector3.up * Mathf.Sin(float_time) * range;
        flash_parent.Find("it_split").localPosition = transform.Find("it_split").localPosition;
        
    }
    [SerializeField]
    Transform root_bone;
    [SerializeField]
    ParticleSystem cloud_ps;
    public void Die()
    {
        Make.The(root_bone).In(.1f).ScaleTo(new Vector3(3f, 0f, 1f)).Happen();
        cloud_ps.Stop();
    }
    public Sequence Attack(System.Action monkey_die_callback, System.Action reset_monkey_stomp)
    {
        return Make.The(root_bone)
            .MakeHappen(() => attacking = true)
            .In(.1f).ScaleTo(new Vector3(.4f, 1.5f, .1f))
            .then.In(.1f).MoveBy(Vector3.up * 15f)
            .then.instantly.MoveTo(FindObjectOfType<Earth>().transform.position + Vector3.up * 15f)
            .then.In(.1f).MoveTo(FindObjectOfType<Earth>().transform.position + Vector3.up * 1f)
            .then.In(.1f).ScaleTo(new Vector3(-1.1f, .9f, .1f))
            .then.In(.1f).ScaleTo(new Vector3(-1f, 1f, 1f))
            .then.MakeHappen(monkey_die_callback)
            .ThenWait(.2f)
            .then.In(.1f).FixedSinTTransition().MoveTo(FindObjectOfType<Earth>().transform.position + Vector3.up * 5f).ScaleTo(new Vector3(-.4f, 1.5f, .1f))
            .then.In(.1f).FixedSinTTransition().MoveTo(FindObjectOfType<Earth>().transform.position + Vector3.up * 1f).ScaleTo(new Vector3(-1f, 1f, 1f))
            .then.MakeHappen(monkey_die_callback)
            .ThenWait(.2f)
            .then.In(.1f).FixedSinTTransition().MoveTo(FindObjectOfType<Earth>().transform.position + Vector3.up * 5f).ScaleTo(new Vector3(-.4f, 1.5f, .1f))
            .then.In(.1f).FixedSinTTransition().MoveTo(FindObjectOfType<Earth>().transform.position + Vector3.up * 1f).ScaleTo(new Vector3(-1f, 1f, 1f))
            .then.MakeHappen(monkey_die_callback)
            .ThenWait(.4f)
            .then.MakeHappen(reset_monkey_stomp)
            .then.In(.1f).FixedSinTTransition().MoveTo(FindObjectOfType<Earth>().transform.position + Vector3.up * 15f).ScaleTo(new Vector3(-.4f, 1.5f, .1f))
            .ThenWait(.2f)
            .then.instantly.MoveTo(orig_root_pos + Vector3.up * 15f)
            .then.In(.15f).MoveTo(orig_root_pos)
            .then.In(.1f).ScaleTo(new Vector3(1.1f, .9f, .1f))
            .then.In(.1f).ScaleTo(Vector3.one)
            .MakeHappen(() => attacking = false);


    }
}
