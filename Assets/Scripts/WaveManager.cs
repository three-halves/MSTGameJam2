using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private float delayBetweenWaves = 4f;

    [SerializeField] private AudioClip bulletSpawnSFX;

    [SerializeField] private GameObject audioObject;

    float timeRef;
    
    // Start is called before the first frame update
    void Start()
    {
        timeRef = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float timeDif = (Mathf.Max(1.5f, delayBetweenWaves)) - (Time.time - timeRef);

        if (timeDif <= 0)
        {
            timeRef = Time.time;
            delayBetweenWaves = Mathf.Max(1.5f, delayBetweenWaves - 0.03f);
            SpawnWave();
        }

        // Debug.Log(timeDif);
    }

    void SpawnWave()
    {
        GameObject.Find("PlayerContainer").GetComponent<Player>().score += 1;
        Wave wave = waves[Random.Range(0, waves.Length)];

        foreach (WaveNode w in wave.nodes)
        {
            for (int i = 0; i < w.bulletSpawnerPrefabs.Length; i++)
            {
                // spawn bullet spawners
                Vector2 posOrigin = (w.centerAtPlayer) ? GameObject.Find("PlayerContainer").transform.position : Vector2.zero;
                BulletSpawner newSpawner = Instantiate(w.bulletSpawnerPrefabs[i]).GetComponent<BulletSpawner>();
                if (!w.randomPosOverride)
                {
                    newSpawner.gameObject.transform.position = posOrigin + (w.pos + w.posDelta * i);
                }
                else
                {
                    newSpawner.gameObject.transform.position = posOrigin + (new Vector2(Random.Range(-13f, 13f), Random.Range(-6.5f, 6.5f)));
                }
                newSpawner.spawnTime = w.delay;
                if (w.randomAngleOverride) w.addAngle = Random.Range(0f, 360f);
                newSpawner.addAngle = w.addAngle;
                newSpawner.audioObject = audioObject;
            }
            
        }
    }
}
