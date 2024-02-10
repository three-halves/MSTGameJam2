using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] public float lifeTime = 4f;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private Sprite[] scoreSprites;

    private float refTime;
    // Start is called before the first frame update
    void Start()
    {
        refTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if ((lifeTime - (Time.time - refTime)) <= 0)
        {
            Bullet newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
            // newBullet.isHoming = true;
            refTime = Time.time;
            GameObject.Find("PlayerContainer").GetComponent<Player>().score += 8;
            Instantiate(scorePrefab, transform.position, transform.rotation).transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = scoreSprites[1];
            Destroy(gameObject);

        }
        else if ((lifeTime - (Time.time - refTime)) <= 1.5)
        {
            Color c = sr.color;
            c.a = 1f - (Mathf.Ceil(Time.time * 6) % 2) * 0.75f;
            sr.color = c;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject.Find("PlayerContainer").GetComponent<Player>().score += 2;
        Instantiate(scorePrefab, transform.position, transform.rotation).transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = scoreSprites[0];
        Destroy(gameObject);
    }
}
