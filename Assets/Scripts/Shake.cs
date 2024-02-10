using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    private float amplitude;

    private float shakeTimer = 0f;
    private Vector2 startPos;
    private Vector2 addPos = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer - Time.time > 0) 
        {
            // Debug.Log("shake " + transform.localPosition);
            addPos = new Vector2(Random.Range(0f, amplitude), Random.Range(0f, amplitude));
            transform.localPosition = startPos + addPos;
        }
    }

    public void StartShake(float a, float d)
    {
        shakeTimer = d + Time.time;
        amplitude = a;
        startPos = transform.localPosition;
    }
}
