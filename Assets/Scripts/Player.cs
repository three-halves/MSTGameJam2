using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Component references
    [Header("Component References")]
    [SerializeField] private HealthDisp hpDisp;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] TMPro.TMP_Text scoreText;
    private CharacterController _characterController;

    // controller input vector
    private Vector3 _rawMoveDelta = new();
    // Transformed input vector based on camera position 
    private Vector3 _moveDelta = new();
    // Input vector stored at start of a roll
    private Vector3 _rollMoveDelta = new();

    // Jump is input but player has not performed jumped yet
    private bool _willJump = false;
    private bool _willRoll = false;
    private bool _rollHeld = false;

    // Player controls constants
    [Header("Control Constants")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _maxRunSpeed;
    [SerializeField] private float _runAccel;
    [SerializeField] private float _runFriction;
    [SerializeField] private float _gravity;
    [SerializeField] private float _jumpHeight;

    [SerializeField] private float _rollSpeed;
    [SerializeField] private float _airRollSpeed;
    [SerializeField] private float _rollDuration;
    [SerializeField] private float _rollCooldown;
    [SerializeField] private float _rollingGravity;
    [SerializeField] private float _rollingJumpHeight;
    
    private bool _groundedLastFrame = false;
    private bool _hitThisFrame = false;
    private bool _isIntangible = false;

    private float _currentRollTimer = 0f;
    private float _intangibleTimer = 0f;

    // Visual references
    [Header("Visual References")]
    [SerializeField] private SpriteRenderer[] playerSprites;
    [SerializeField] private Sprite bootWalkSprite;
    [SerializeField] private Sprite bootJumpSprite;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private ParticleSystem deathParicles;

    [SerializeField] private Shake shake;
    private Vector3 startingScale;
    private SpriteRenderer playerHeadSR;

    [NonSerialized] public int score = 0;
    [NonSerialized] public float timeSurvived = 0;

    // Sound effects
    [Header("Sound Effects")]
    [SerializeField] GameObject dashSFX;
    [SerializeField] GameObject stepSFX;
    [SerializeField] GameObject hurtSFX;
    [SerializeField] GameObject deathSFX;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        startingScale = playerVisual.localScale;
        playerHeadSR = playerVisual.transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // Amount we will move player at the end of tick
        Vector3 delta;

        // running movement
        if (_rollHeld && !_willRoll && _currentRollTimer <= 0)
        {
            // move player with acceleration
            delta = GetMovementVector(_moveDelta, _characterController.velocity);
        }
        // If not running, do basic walk/roll controls
        else
        {
            delta = _moveDelta * _walkSpeed;
            if (_currentRollTimer > 0)
            {
                delta = _rollMoveDelta * (_characterController.isGrounded ? _rollSpeed : _airRollSpeed);
            }
        }

        // jumping and gravity
        delta.y = _characterController.velocity.y * Time.fixedDeltaTime;
        if (_willJump && _characterController.isGrounded)
        {
            delta.y = _currentRollTimer <= 0 ? _jumpHeight : _rollingJumpHeight;
            _willJump = false;
        }
        delta.y -= _currentRollTimer <= 0 ? _gravity : _rollingGravity;

        _characterController.Move(delta);
    }

    // Update is called once per frame
    void Update()
    {
        if (hpDisp.hp == 0) return;
        timeSurvived += Time.deltaTime;

        // start a roll
        if (_willRoll && _currentRollTimer <= -_rollCooldown && _moveDelta != Vector3.zero)
        {
            _currentRollTimer = _rollDuration;
            _intangibleTimer = _rollDuration;
            _rollMoveDelta = _moveDelta;
            _willRoll = false;
            Instantiate(dashSFX);
        }

        _isIntangible = _intangibleTimer > 0;
        // continue dash in air
        if (_characterController.isGrounded) 
        {
            _currentRollTimer -= Time.deltaTime;
            _intangibleTimer -= Time.deltaTime;
        }
        // stop air dash on land
        if (_characterController.isGrounded && !_groundedLastFrame)
        {
            _currentRollTimer = 0f;
            _intangibleTimer = 0f;
        }

        UpdateVisuals();

        _hitThisFrame = false;
        _groundedLastFrame = _characterController.isGrounded;
    }

    // update player visuals and animation variables
    private void UpdateVisuals()
    {
        // Change head color to indicate iframes
        if (_isIntangible) playerHeadSR.color = Color.green;
        else playerHeadSR.color = Color.white;

        if (_moveDelta.x != 0) playerVisual.transform.localScale = new Vector3((_moveDelta.x > 0) ? startingScale.x : -startingScale.x, startingScale.y, startingScale.z);
        animator.SetBool("Walk", (_moveDelta != Vector3.zero) && _currentRollTimer <= 0 && _characterController.isGrounded && !_rollHeld);
        animator.SetBool("Roll", _currentRollTimer > 0);
        animator.SetBool("Run", _rollHeld && !_willRoll && _currentRollTimer <= 0 && (_moveDelta != Vector3.zero));
        animator.SetBool("InAir", !_characterController.isGrounded);
        animator.SetFloat("YVel", _characterController.velocity.y);

        // shoe sprite
        Sprite s = _characterController.isGrounded ? bootWalkSprite : bootJumpSprite;
        // shoe layer order, either behind or in front of head
        int o = (_characterController.isGrounded || _currentRollTimer > 0) ? 1 : 3;
        playerSprites[0].sprite = s;
        playerSprites[1].sprite = s;
        playerSprites[0].sortingOrder = o;
        playerSprites[1].sortingOrder = o;
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
                float d = lateralVel.magnitude * _runFriction * Time.fixedDeltaTime;
                currentVel.x *= Mathf.Max(lateralVel.magnitude - d, 0) / lateralVel.magnitude;
                currentVel.z *= Mathf.Max(lateralVel.magnitude - d, 0) / lateralVel.magnitude;
            }

            return AddAcceleration(
                inputDir,
                currentVel,
                _runAccel / Time.fixedDeltaTime,
                _maxRunSpeed / Time.fixedDeltaTime
                );
        }

    public bool Hurt(bool ignoresIntangible = false)
    {
        if ((_isIntangible && !ignoresIntangible) || _hitThisFrame) return false;
        _hitThisFrame = true;
        hpDisp.hp -= 1;
        hpDisp.hpObjects[hpDisp.hp].SetActive(false);
        hpDisp.timeSinceHit = Time.time;
        if (hpDisp.hp <= 0) 
        {
            Death();
            return true;
        }

        _intangibleTimer = 1f;
        shake.StartShake(0.3f, 0.3f);
        score = (int)Mathf.Ceil(score * 0.75f);
        Instantiate(hurtSFX);

        return true;
    }

    private void Death()
    {
        playerHeadSR.color = Color.clear;
        scoreText.text = "";
        _moveDelta = Vector2.zero;
        animator.SetBool("Walk", false);
        deathParicles.Play();
        Instantiate(deathSFX);
        StartCoroutine(GameObject.Find("PostGameParent").GetComponent<PostGameWindow>().StartPostGame());
    }

    public void CalculateMoveInputDir()
    {
        if (cameraTransform == null) return;
        _moveDelta = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * _rawMoveDelta;
    }

    void OnMove(InputValue value)
    {
        var v = value.Get<Vector2>();
        _rawMoveDelta = new Vector3(v.x, 0, v.y).normalized;
        CalculateMoveInputDir();
    }

    void OnJump(InputValue v)
    {
        _willJump = v.isPressed;
    }

    void OnRoll(InputValue v)
    {
        _willRoll = v.isPressed;
        _rollHeld = v.isPressed;
    }
}
