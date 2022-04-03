using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Earth : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    [SerializeField]
    SpriteRenderer tex, clouds;
    [SerializeField]
    float tex_speed = 6f, clouds_speed = 1f, rotation_speed = 90f;
    // Update is called once per frame
    void Update()
    {
        tex.size += Vector2.right * Time.deltaTime * tex_speed;
        clouds.size += Vector2.right * Time.deltaTime * clouds_speed;
        //clouds.size += Vector2.up * Time.deltaTime * clouds_speed * .3f;
        tex.transform.Rotate(Vector3.forward * rotation_speed * Time.deltaTime);
        if(tex.size.x > 18f)
        {
            tex.size -= Vector2.right * 12f;
        }
        if (clouds.size.x > 18f)
        {
            clouds.size -= Vector2.right * 12f;
        }
        if (clouds.size.y > 15f)
        {
            clouds.size -= Vector2.up * 10f;
        }
        transform.Find("surface_container").Rotate(Vector3.forward * Time.deltaTime * 18f);
    }
}
