using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 moveDelta = new Vector2();
    private CharacterController cc;
    private bool isJumping = false;
    private bool isRolling = false;
    private bool rollHeld = false;
    [SerializeField] private float moveSpd;
    [SerializeField] private float maxSpd;
    [SerializeField] private float runSpd;
    [SerializeField] private float friction;
    [SerializeField] private float rollSpd;
    [SerializeField] private float airRollSpd;
    [SerializeField] private float rollTime;
    [SerializeField] private float gravity;
    [SerializeField] private float rollingGravity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float rollingJumpHeight;
    private bool groundedLastFrame = false;
    private Vector2 dashMoveDelta;
    private float curDashTime = 0f;
    [SerializeField] private float dashCooldown;
    private bool intangible = false;
    private float intangibleTime = 0f;
    [SerializeField] private HealthDisp hpDisp;

    private bool hitThisFrame = false;
    [SerializeField] SpriteRenderer[] playerSprites;
    [SerializeField] private Sprite bootWalkSprite;
    [SerializeField] private Sprite bootJumpSprite;
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

    [SerializeField] TMPro.TMP_Text scoreText;

    private SpriteRenderer playerHeadSR;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        startingScale = playerVisual.localScale;
        playerHeadSR = playerVisual.transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // Lateral input direction
        Vector3 v = new Vector3(moveDelta.x, 0, moveDelta.y).normalized;
        // Amount we will move player at the end of tick
        Vector3 delta = Vector3.zero;

        // running movement
        if (rollHeld && !isRolling && curDashTime <= 0)
        {
            // move player with acceleration
            delta = GetMovementVector(v, cc.velocity);
        }
        // If not running, do basic walk/roll controls
        else
        {
            delta = v * moveSpd;
            if (curDashTime > 0)
            {
                delta = new Vector3(dashMoveDelta.x, 0, dashMoveDelta.y).normalized * (cc.isGrounded ? rollSpd : airRollSpd);
            }
        }

        // jumping and gravity
        delta.y = cc.velocity.y * Time.deltaTime;
        if (isJumping && cc.isGrounded)
        {
            delta.y = curDashTime <= 0 ? jumpHeight : rollingJumpHeight;
            isJumping = false;
        }

        delta.y -= curDashTime <= 0 ? gravity : rollingGravity;

        cc.Move(delta);
    }

    // Update is called once per frame
    void Update()
    {
        if (hpDisp.hp == 0) return;
        timeSurvived += Time.deltaTime;

        // start a roll
        if (isRolling && curDashTime <= -dashCooldown && moveDelta != Vector2.zero)
        {
            curDashTime = rollTime;
            intangibleTime = rollTime;
            dashMoveDelta = moveDelta.normalized;
            isRolling = false;
            Instantiate(dashSFX);
        }

        intangible = intangibleTime > 0;
        // continue dash in air
        if (cc.isGrounded) 
        {
            curDashTime -= Time.deltaTime;
            intangibleTime -= Time.deltaTime;
        }
        // stop air dash on land
        if (cc.isGrounded && !groundedLastFrame)
        {
            curDashTime = 0f;
            intangibleTime = 0f;
        }

        // temp for debug
        if (intangible) playerHeadSR.color = Color.green;
        else playerHeadSR.color = Color.white;

        // update player visuals and animations
        if (moveDelta.x != 0) playerVisual.transform.localScale = new Vector3((moveDelta.x > 0) ? startingScale.x : -startingScale.x, startingScale.y, startingScale.z);
        animator.SetBool("Walk", (moveDelta != Vector2.zero) && curDashTime <= 0 && cc.isGrounded && !rollHeld);
        animator.SetBool("Roll", curDashTime > 0);
        animator.SetBool("Run", rollHeld && !isRolling && curDashTime <= 0 && (moveDelta != Vector2.zero));
        animator.SetBool("InAir", !cc.isGrounded);
        animator.SetFloat("YVel", cc.velocity.y);

        // shoe sprite
        Sprite s = cc.isGrounded ? bootWalkSprite : bootJumpSprite;
        // shoe layer order
        int o = (cc.isGrounded || curDashTime > 0) ? 1 : 3;
        playerSprites[0].sprite = s;
        playerSprites[1].sprite = s;
        playerSprites[0].sortingOrder = o;
        playerSprites[1].sortingOrder = o;

        hitThisFrame = false;
        groundedLastFrame = cc.isGrounded;
        scoreText.text = score.ToString();

    }

    private Vector3 AddAcceleration(Vector3 inputDir, Vector3 currentVel, float acceleration, float maxVel)
    {
        float projectedVel = Vector3.Dot(currentVel * Time.fixedDeltaTime, inputDir);
        float accelVel = acceleration * Time.fixedDeltaTime;
        maxVel *= Time.fixedDeltaTime;

        // Cap max accel
        if (projectedVel + accelVel > maxVel)
        {
            accelVel = maxVel - projectedVel;
        }
        
        return currentVel * Time.fixedDeltaTime + inputDir * accelVel;
    }

    private Vector3 GetMovementVector(Vector3 inputDir, Vector3 currentVel)
        {
            // Apply friction
            Vector3 lateralVel = Vector3.Scale(currentVel, new Vector3(1, 0, 1));
            if (lateralVel.magnitude != 0)
            {
                float d = lateralVel.magnitude * friction * Time.fixedDeltaTime;
                currentVel.x *= Mathf.Max(lateralVel.magnitude - d, 0) / lateralVel.magnitude;
                currentVel.z *= Mathf.Max(lateralVel.magnitude - d, 0) / lateralVel.magnitude;
            }

            return AddAcceleration(
                inputDir,
                currentVel,
                runSpd,
                maxSpd
                );
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
        scoreText.text = "";
        moveDelta = Vector2.zero;
        animator.SetBool("Walk", false);
        deathParicles.Play();
        Instantiate(deathSFX);
        StartCoroutine(GameObject.Find("PostGameParent").GetComponent<PostGameWindow>().StartPostGame());
    }

    void OnMove(InputValue v)
    {
        moveDelta = v.Get<Vector2>();
    }

    void OnJump(InputValue v)
    {
        isJumping = v.isPressed;
    }

    void OnRoll(InputValue v)
    {
        isRolling = v.isPressed;
        rollHeld = v.isPressed;
    }
}
