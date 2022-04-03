using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    //bool initialize = true;
    void Start()
    {
        for(int i = 0; i < hearts_number; i++)
        {
            float t = ((float)i) / hearts_number;
            Transform heart = Instantiate(gameObject).transform;
            Destroy(heart.GetComponent<Health>());
            
            hearts.Add(heart.GetComponent<SpriteRenderer>());
            heart.localScale = Vector2.Lerp(min_scale, max_scale, t);
            
            heart.GetComponent<SpriteRenderer>().sortingOrder = hearts_number - i;
            heart.transform.localScale = Vector2.Lerp(min_scale, max_scale, t);
            heart.GetComponent<SpriteRenderer>().color = Color.Lerp(min_color, max_color, t);
        }
        hearts.ForEach(sr => { sr.transform.SetParent(transform); sr.transform.localPosition = Vector2.zero; });;
        GetComponent<SpriteRenderer>().enabled = false;
        display_health = health;
    }
    [SerializeField]
    public float health = 1f, approach_speed = 3f;
    float display_health = 1f;
    [SerializeField]
    Vector2 min_scale, max_scale;
    [SerializeField]
    Color min_color, max_color;
    List<SpriteRenderer> hearts = new List<SpriteRenderer>();
    [SerializeField]
    int hearts_number = 5;
    // Update is called once per frame
    void Update()
    {
        display_health = Mathf.MoveTowards(display_health, health, Time.deltaTime * approach_speed);
        for(int i = 0; i < hearts_number; i++)
        {
            SpriteRenderer sr = hearts[i];
                
            Color c = sr.color;
            c.a = GetAlpha(i);
            sr.color = c;
            
        }
        
    }
    float GetAlpha(int i)
    {
        float step = 1f / hearts_number;
        return Mathf.InverseLerp(i * step, (i + 1) * step, display_health);
        
    }
}
