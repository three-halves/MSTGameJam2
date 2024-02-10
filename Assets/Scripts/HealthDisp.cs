using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisp : MonoBehaviour
{

    [SerializeField] public GameObject[] hpObjects;
    public int hp;
    public float timeSinceHit;

    // time the hpbar shows before fading
    [SerializeField] private float stayTime = 2f;
    [SerializeField] private float fadeTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        hp = hpObjects.Length;
        timeSinceHit = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // hpbar opacity
        foreach (GameObject o in hpObjects)
        {
            SpriteRenderer sr = o.GetComponent<SpriteRenderer>();
            Color c = sr.color;
            c.a = ((Time.time - timeSinceHit < stayTime) ? 1f : Mathf.Max(0.1f, 1f - ((Time.time - timeSinceHit) - stayTime) * fadeTime));
            sr.color = c;
        }
    }
}
