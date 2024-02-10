using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    // passed in as (Magnitude, Angle) and converted to typical velocity vector
    [SerializeField] Vector2[] bulletVectors;
    // bullet objects created based on velocity list
    private Vector2[] bulletVels;

    // time between apearing and spawning bullets
    [SerializeField] public float spawnTime = 3f;
    // private float spawnTime;

    [SerializeField] private Sprite[] countdownSprites;
    [SerializeField] private SpriteRenderer countdownRenderer;

    [SerializeField] private GameObject lineRenderPrefab;
    [SerializeField] private GameObject bulletPrefab;

    // passed from WaveManager
    public float addAngle = 0f;

    private float refTime;
    // Start is called before the first frame update
    void Start()
    {
        Color c = countdownRenderer.color;
        c.a = 0.5f;
        countdownRenderer.color = c;
        // spawnTime = startSpawnTime;
        refTime = Time.time;

        // create velocity vectors (bulletVels) by using magnitude and angle (contained in bulletVectors)
        bulletVels = new Vector2[bulletVectors.Length];
        for (int i = 0; i < bulletVectors.Length; i++)
        {
            bulletVels[i] = new Vector2(bulletVectors[i].x * Mathf.Cos((bulletVectors[i].y + addAngle) * Mathf.Deg2Rad), bulletVectors[i].x * Mathf.Sin((bulletVectors[i].y + addAngle) * Mathf.Deg2Rad));
        }

        // setup bullet warning lines
        foreach (Vector2 v in bulletVels)
        {
            LineRenderer newLR = Instantiate(lineRenderPrefab, transform).GetComponent<LineRenderer>();
            newLR.SetPosition(0, transform.position);
            newLR.SetPosition(1, (Vector2) transform.position + v.normalized);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float timeLeft = Mathf.Ceil(spawnTime - (Time.time - refTime));

        // Debug.Log(timeLeft);

        if (timeLeft > 0)
        {
            countdownRenderer.sprite = countdownSprites[(int) timeLeft - 1];
        }
        else
        {
            Spawn();
        }
        
    }

    void Spawn()
    {
        foreach (Vector2 v in bulletVels)
        {
            Bullet newBullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
            newBullet.vel = v;
            newBullet.gameObject.transform.position = transform.position;
        }
        Destroy(gameObject);
    }
}
