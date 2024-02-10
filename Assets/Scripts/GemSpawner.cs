using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    private float refTime;
    [SerializeField] Vector2 spawnTimeRange;
    private float spawnTime;
    [SerializeField] private GameObject gemPrefab;

    [SerializeField] private Vector2 xPosRange;
    [SerializeField] private Vector2 yPosRange;
    // Start is called before the first frame update
    void Start()
    {
        refTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTime - (Time.time - refTime) <= 0)
        {
            Spawn();
            refTime = Time.time;
            spawnTime = Random.Range(spawnTimeRange.x, spawnTimeRange.y);
        }
    }

    void Spawn()
    {
        GameObject newGem = Instantiate(gemPrefab);
        newGem.transform.position = new Vector3(Random.Range(xPosRange.x * 1f, xPosRange.y), Random.Range(yPosRange.x * 1f, yPosRange.y), 0f);
    }
}
