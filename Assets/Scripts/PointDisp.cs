using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDisp : MonoBehaviour
{
    [SerializeField] float lifeSpan = 0.5f;
    private float refTime;
    // Start is called before the first frame update
    void Start()
    {
        refTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeSpan - (Time.time - refTime) < 0)
        {
            Destroy(gameObject);
        }
    }
}
