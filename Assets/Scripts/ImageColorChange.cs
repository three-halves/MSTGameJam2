using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random=UnityEngine.Random;

public class ImageColorChange : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float offset = -1f;
    // Start is called before the first frame update
    void Start()
    {
        if (offset < 0) offset = Random.Range(0, (float) (2 * Math.PI));
    }
    // Update is called once per frame
    void Update()
    {
        Color c = image.color;

        c.r = Nsin((float)(Time.time * speed + offset)) * 0.5f + 0.5f;
        c.b = Nsin((float)(Time.time * speed + offset + Math.PI / 2)) * 0.5f + 0.5f;
        c.g = Nsin((float)(Time.time * speed + offset + Math.PI)) * 0.5f + 0.5f;

        image.color = c;
    }

    float Nsin(float v)
    {
        return ((Mathf.Sin(v) + 1f) / 2f);
    }
}
