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
    // Update is called once per frame
    void Update()
    {
        transform.position = flash_parent.position = startpos + Vector3.up * Mathf.Sin(Time.time) * speed * range;
        flash_parent.Find("it_split").localPosition = transform.Find("it_split").localPosition;
    }
    [SerializeField]
    Transform root_bone;
    [SerializeField]
    ParticleSystem cloud_ps;
}
