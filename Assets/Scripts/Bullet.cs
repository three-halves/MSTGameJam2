using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Rigidbody2D rb;

    public Vector2 vel;
    private Vector2 accel = Vector2.zero;

    private Renderer rend;

    public bool isHoming = false;
    private float homingSpeed = 0.8f;
    private float homingLifetime = 5f;

    [SerializeField] private Sprite[] sprites;

    private float refTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<Renderer>();

        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, (sprites.Length - 1))];
        transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)));
        refTime = Time.time;
    }

    void FixedUpdate()
    {
        vel += accel;
        if (isHoming) vel *= 0.97f;
        rb.linearVelocity = vel;
    }

    // Update is called once per frame
    void Update()
    {
        if(!rend.isVisible && !isHoming)
        {
            Destroy(gameObject);
        }

        // point accel toward player
        if (isHoming)
        {
            Transform playerTransform = GameObject.Find("PlayerContainer").transform;
            Vector2 playerPos = playerTransform.position;
            // rotate toward player for a set duration
            if (Time.time - refTime < homingLifetime)
            {
                float theta = Mathf.Atan2(playerPos.y - transform.position.y, playerPos.x - transform.position.x);
                accel = new Vector2(homingSpeed * Mathf.Cos(theta), homingSpeed * Mathf.Sin(theta));
                transform.rotation = Quaternion.Euler(0,0, Mathf.Atan2(playerPos.y - transform.position.y, playerPos.x - transform.position.x) * Mathf.Rad2Deg);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            if (player.Hurt()) Destroy(gameObject);
        }
    }
}
