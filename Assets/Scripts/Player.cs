using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Player : MonoBehaviour
{
    private Vector2 moveDelta = new Vector2();
    private Rigidbody2D rb;


    [SerializeField] private float moveSpd;
    [SerializeField] private float dashSpd;
    [SerializeField] private float dashTime = 1f;
    private Vector2 dashMoveDelta;
    private float curDashTime = 0f;
    [SerializeField] private float dashCooldown;
    private bool intangible = false;
    private float intangibleTime = 0f;
    [SerializeField] private HealthDisp hpDisp;

    private bool hitThisFrame = false;

    [SerializeField] SpriteRenderer[] playerSprites;
    private Vector3 startingScale;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private ParticleSystem deathParicles;

    [SerializeField] private Shake shake;

    public int score = 0;
    public float timeSurvived = 0;

    [SerializeField] GameObject dashSFX;
    [SerializeField] GameObject stepSFX;
    [SerializeField] GameObject hurtSFX;
    [SerializeField] GameObject deathSFX;

    private SpriteRenderer playerHeadSR;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingScale = playerVisual.localScale;
        playerHeadSR = playerVisual.transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        Vector2 v = rb.linearVelocity;

        v = moveDelta * moveSpd;

        if (curDashTime > 0)
        {
            v = dashMoveDelta * dashSpd;
        }

        rb.linearVelocity = v;
    }

    // Update is called once per frame
    void Update()
    {
        if (hpDisp.hp == 0) return;
        timeSurvived += Time.deltaTime;

        if (curDashTime <= 0)
        {
            moveDelta.y = Input.GetAxisRaw("Vertical");
            moveDelta.x = Input.GetAxisRaw("Horizontal");
        }

        if (Input.GetButton("Jump") && curDashTime <= -dashCooldown && moveDelta != Vector2.zero)
        {
            curDashTime = dashTime;
            intangibleTime = dashTime;
            dashMoveDelta = moveDelta.normalized;
            Instantiate(dashSFX);
        }

        intangible = intangibleTime > 0;
        curDashTime -= Time.deltaTime;

        // temp for debug
        if (intangible) playerHeadSR.color = Color.green;
        else playerHeadSR.color = Color.white;

        // update player visuals and animations
        if (moveDelta.x != 0) playerVisual.transform.localScale = new Vector3((moveDelta.x > 0) ? startingScale.x : -startingScale.x, startingScale.y, startingScale.z);
        animator.SetBool("Walk", (moveDelta != Vector2.zero) && curDashTime <= 0);
        animator.SetBool("Roll", curDashTime > 0);

        intangibleTime -= Time.deltaTime;
        hitThisFrame = false;
    }

    public bool Hurt(bool ignoresIntangible = false)
    {
        if ((intangible && !ignoresIntangible) || hitThisFrame) return false;
        hitThisFrame = true;
        hpDisp.hp -= 1;
        hpDisp.hpObjects[hpDisp.hp].SetActive(false);
        hpDisp.timeSinceHit = Time.time;
        if (hpDisp.hp <= 0) 
        {
            Death();
            return true;
        }

        intangibleTime = 1f;
        shake.StartShake(0.3f, 0.3f);
        score = (int)Mathf.Ceil(score * 0.75f);
        Instantiate(hurtSFX);

        return true;
    }

    private void Death()
    {
        playerHeadSR.color = Color.clear;
        moveDelta = Vector2.zero;
        animator.SetBool("Walk", false);
        deathParicles.Play();
        Instantiate(deathSFX);
        StartCoroutine(GameObject.Find("PostGameParent").GetComponent<PostGameWindow>().StartPostGame());
    }
}
