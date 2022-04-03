using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IEnumRunner;
using IEnumRunner.Transitions;

public class Monkey : MonoBehaviour
{
    [SerializeField]
    float move_speed;
    [SerializeField]
    Transform body;
    Vector3 orig_scale;
    static bool first_monkey = true;
    public static bool monkeys_moving = true;
    private void Start()
    {
        transform.position = FindObjectOfType<Earth>().transform.position;
        StartCoroutine(Move());
        z = first_monkey ? (-transform.parent.localRotation.eulerAngles.z + 31.587f) : Random.Range(0f, 360f);
        first_monkey = false;
        transform.localRotation = Quaternion.Euler(Vector3.forward * z);
        orig_scale = body.localScale;
        body.localScale = Vector3.zero;
        Make.The(body).In(.1f).ScaleTo(Common.MultiplyVectorBy(new Vector3(.8f, 1.2f, .1f), orig_scale))
            .then.In(.1f).ScaleTo(Common.MultiplyVectorBy(orig_scale, new Vector3(1.2f, .8f, 1f)))
            .then.In(.1f).ScaleTo(orig_scale).Happen();
    }
    private void Update()
    {
        transform.localRotation = Quaternion.Euler(Vector3.forward * z);
    }
    Animator anim => GetComponentInChildren<Animator>();
    float z;
    [SerializeField]
    FloatRange move_delay;
    IEnumerator Move()
    {
        while (true)
        {
            if (first_monkey)
            {
                yield return new WaitForSeconds(6f);
            }
            while (!monkeys_moving)
            {
                yield return null;
            }
            anim.SetBool("move", false);
            yield return new WaitForSeconds(move_delay);
            anim.SetBool("move", true);
            float new_z = z + Random.Range(45f, 120f) * Common.EitherOr();
            GetComponentInChildren<SpriteRenderer>().flipX = new_z < z;
            while (!Mathf.Approximately(new_z, z))
            {
                z = Mathf.MoveTowards(z, new_z, Time.deltaTime * move_speed);
                yield return null;
            }
            
        }
    }
}
