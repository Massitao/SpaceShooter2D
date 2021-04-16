﻿using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ship : MonoBehaviour, IDamageable
{
    #region Variables
    #region Components
    [Header("Components")]
    [SerializeField] private Collider2D     shipCollider;
    [SerializeField] private PlayerInput    shipInput;
    [SerializeField] private Animator       shipAnim;

    [Header("Child Components")]
    [SerializeField] private Animator leftEngineAnim;
    [SerializeField] private Animator rightEngineAnim;
    [SerializeField] private Animator shieldAnim;
    #endregion

    #region Animator Strings
    [Header("Animator References")]
    // SHIP
    [SerializeField] private string shipAnim_Input;
    private int shipAnim_InputHash => Animator.StringToHash(shipAnim_Input);

    [SerializeField] private string shipAnim_Invincibility;
    private int shipAnim_InvincibilityHash => Animator.StringToHash(shipAnim_Invincibility);

    // ENGINES
    [SerializeField] private string engineAnim_Hurt;
    private int engineAnim_HurtHash => Animator.StringToHash(engineAnim_Hurt);

    // SHIELD
    [SerializeField] private string shieldAnim_Health;
    private int shieldAnim_HealthHash => Animator.StringToHash(shieldAnim_Health);
    #endregion

    #region Input Values
    [Header("Inputs")]
    private Vector2 moveInput;
    private bool doShoot;
    private bool isThrusting;
    #endregion

    #region Ship Properties
    [Header("Ship Health")]
    [SerializeField] private int entityHealth;
    public int EntityHealth { get { return entityHealth; } set { entityHealth = value; } }

    [SerializeField] private int entityMaxHealth = 3;
    public int EntityMaxHealth { get { return entityMaxHealth; } set { entityMaxHealth = value; } }


    [Header("Ship Explosion")]
    [SerializeField] private GameObject explosion;


    [Header("Ship Move")]
    [SerializeField] private float shipSpeed = 8f;
    [SerializeField] private float shipThrusterSpeed = 10f;


    [Header("Ship Shoot")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Vector3 laserSpawnOffset;
    [SerializeField] private float fireRate = .2f;
    private float fireRateTimer;

    [SerializeField] private int ammoCount;
    [SerializeField] private int maxAmmo;

    [SerializeField] [Range(0, 30)] private int shipProjectileLayer;


    [Header("Ship Invincibility")]
    private WaitForSeconds invincibilityDuration = new WaitForSeconds(2f);
    private Coroutine invincibilityCoroutine;
    private bool invincible => invincibilityCoroutine != null;


    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip noAmmoClip;
    [SerializeField] private AudioClip explosionClip;
    #endregion

    #region PowerUps
    [Header("Abilities")]
    [SerializeField] private int shootAbilityAmmoRefill = 10;


    [Header("Triple Shoot")]
    [SerializeField] private GameObject tripleShotPrefab;

    private Coroutine tripleShotCoroutine;
    private WaitForSeconds tripleShotDuration = new WaitForSeconds(5f);
    private bool tripleShotActive => tripleShotCoroutine != null;


    [Header("Heat Seek Shoot")]
    [SerializeField] private GameObject heatSeekShotPrefab;

    private Coroutine heatSeekShotCoroutine;
    private WaitForSeconds heatSeekDuration = new WaitForSeconds(5f);
    private bool heatSeekShotActive => heatSeekShotCoroutine != null;


    [Header("Extra Speed")]
    [SerializeField] [Range(1f, 3f)] private float extraSpeedMultiplier;

    private Coroutine extraSpeedCoroutine;
    private WaitForSeconds extraSpeedDuration = new WaitForSeconds(8f);
    private bool extraSpeedActive => extraSpeedCoroutine != null;


    [Header("Shield")]
    private int shieldHealth = 0;
    private int shieldMaxHealth = 3;
    private bool shieldActive;
    #endregion

    #region Events
    // Events
    public event System.Action<int> OnShoot;
    public event System.Action<int> OnAmmoRefill;
    public event System.Action<int> OnEntityHealed;
    public event System.Action<int> OnEntityDamaged;
    public event System.Action<IDamageable> OnEntityKilled;
    #endregion
    #endregion


    #region MonoBehaviour Methods
    private void Awake()
    {
        entityHealth = entityMaxHealth;
        ammoCount = maxAmmo;
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        Shoot();
    }
    #endregion

    #region Custom Methods
    #region Input
    public void UpdateMoveInput(InputAction.CallbackContext input)
    {
        // Get Player Input
        moveInput = input.ReadValue<Vector2>();

        // Update Ship Move Animations
        shipAnim.SetFloat(shipAnim_InputHash, moveInput.x);
    }
    public void UpdateShootInput(InputAction.CallbackContext input)
    {
        doShoot = input.ReadValueAsButton();

        if (input.performed && ammoCount == 0)
        {
            AudioManager.Instance?.PlayOneShotClip(noAmmoClip);
        }
    }
    public void UpdateThrusterInput(InputAction.CallbackContext input)
    {
        isThrusting = input.ReadValueAsButton();
    }
    #endregion

    private void Move()
    {
        // If Player is using the Thrusters, apply Thruster speed. Else, keep normal ship speed
        float selectedSpeed = isThrusting ? shipThrusterSpeed : shipSpeed;

        // If Extra Speed Power-up is active, apply extra speed. Else, keep selected Speed
        selectedSpeed *= extraSpeedActive ? extraSpeedMultiplier : 1f;

        transform.Translate(moveInput * selectedSpeed * Time.deltaTime);

        // Wrap ship around X axis
        if (Mathf.Abs(transform.position.x) > SpaceShooterData.PlayerStartWrapX)
        {
            transform.position = new Vector3(SpaceShooterData.PlayerStartWrapX * Mathf.Sign(transform.position.x) * -1, transform.position.y);
        }

        // Clamp Y position so the player doesn't leave the screen
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, SpaceShooterData.PlayerBoundLimitsY.x, SpaceShooterData.PlayerBoundLimitsY.y), transform.position.z);
    }
    private void Shoot()
    {
        if (doShoot && ammoCount != 0)
        {
            if (Time.time >= fireRateTimer)
            {
                // Subtract laser
                ammoCount--;

                // Update timer
                fireRateTimer = Time.time + fireRate;

                // Choose between Triple, Heat Seek or Single laser
                GameObject laserType = laserPrefab;
                laserType = tripleShotActive ? tripleShotPrefab : laserType;
                laserType = heatSeekShotActive ? heatSeekShotPrefab : laserType;

                // Ignore own lasers
                Transform[] lasersToIgnore = Instantiate(laserType, transform.position + laserSpawnOffset, Quaternion.identity).GetComponentsInChildren<Transform>();
                for (int i = 0; i < lasersToIgnore.Length; i++)
                {
                    lasersToIgnore[i].gameObject.layer = shipProjectileLayer;
                }

                // Invoke event
                OnShoot?.Invoke(ammoCount);

                // Play Shoot soundclip
                AudioManager.Instance?.PlayOneShotClip(shootClip);
            }
        }
    }

    public int GetAmmoCount()
    {
        return ammoCount;
    }
    public int GetMaxAmmoCount()
    {
        return maxAmmo;
    }

    #region PowerUps
    /// <summary>
    /// Triggers a ship ability.
    /// </summary>
    /// <param name="powerup">Ability to Trigger.</param>
    public void ActivatePowerUp(PowerUp.Type powerup)
    {
        // Instead of having multiple methods, start every Coroutine here
        switch (powerup)
        {
            case PowerUp.Type.TripleShot:
                if (heatSeekShotCoroutine != null) StopPowerUp(PowerUp.Type.HeatSeek);
                if (tripleShotCoroutine != null) StopPowerUp(PowerUp.Type.TripleShot);

                tripleShotCoroutine = StartCoroutine(AbilityDuration(() => StopPowerUp(PowerUp.Type.TripleShot), tripleShotDuration));
                RefillAmmo(shootAbilityAmmoRefill);
                break;

            case PowerUp.Type.HeatSeek:
                if (tripleShotCoroutine != null) StopPowerUp(PowerUp.Type.TripleShot);
                if (heatSeekShotCoroutine != null) StopPowerUp(PowerUp.Type.HeatSeek);

                heatSeekShotCoroutine = StartCoroutine(AbilityDuration(() => StopPowerUp(PowerUp.Type.HeatSeek), heatSeekDuration));
                RefillAmmo(shootAbilityAmmoRefill);
                break;

            case PowerUp.Type.Speed:
                if (extraSpeedCoroutine != null) StopPowerUp(PowerUp.Type.TripleShot);

                extraSpeedCoroutine = StartCoroutine(AbilityDuration(() => StopPowerUp(PowerUp.Type.Speed), extraSpeedDuration));
                break;

            case PowerUp.Type.Shield:
                ActivateShield();
                break;

            case PowerUp.Type.Life:
                HealShip(1);
                break;

            case PowerUp.Type.Ammo:
                RefillAmmo();
                break;
        }
    }
    /// <summary>
    /// Interrupts a ship ability, if it's active.
    /// </summary>
    /// <param name="powerup">Ability to Stop.</param>
    private void StopPowerUp(PowerUp.Type powerup)
    {
        switch (powerup)
        {
            case PowerUp.Type.TripleShot:
                if (tripleShotCoroutine != null)
                {
                    StopCoroutine(tripleShotCoroutine);
                    tripleShotCoroutine = null;
                }
                break;

            case PowerUp.Type.HeatSeek:
                if (heatSeekShotCoroutine != null)
                {
                    StopCoroutine(heatSeekShotCoroutine);
                    heatSeekShotCoroutine = null;
                }
                break;

            case PowerUp.Type.Speed:
                if (extraSpeedCoroutine != null)
                {
                    StopCoroutine(extraSpeedCoroutine);
                    extraSpeedCoroutine = null;
                }
                break;

            case PowerUp.Type.Shield:
                StopShield();
                break;
        }
    }

    private IEnumerator AbilityDuration(System.Action OnEndCallback, WaitForSeconds abilityDuration)
    {
        yield return abilityDuration;
        OnEndCallback?.Invoke();
    }

    private void ActivateShield()
    {
        shieldHealth = shieldMaxHealth;
        shieldActive = true;

        shieldAnim.gameObject.SetActive(shieldActive);

        shieldAnim.SetInteger(shieldAnim_HealthHash, shieldHealth);
    }
    private void StopShield()
    {
        if (shieldActive)
        {
            shieldHealth = 0;
            shieldActive = false;

            shieldAnim.SetInteger(shieldAnim_HealthHash, shieldHealth);
            shieldAnim.gameObject.SetActive(shieldActive);
        }
    }

    private void HealShip(int ammountToHeal)
    {
        ammountToHeal = Mathf.Clamp(ammountToHeal, 0, EntityMaxHealth);
        EntityHealth = Mathf.Clamp(EntityHealth + ammountToHeal, 0, EntityMaxHealth);

        leftEngineAnim.SetBool(engineAnim_Hurt, EntityHealth < 3);
        rightEngineAnim.SetBool(engineAnim_Hurt, EntityHealth < 2);

        OnEntityHealed?.Invoke(EntityHealth);
    }
    private void RefillAmmo()
    {
        ammoCount = maxAmmo;
        OnAmmoRefill?.Invoke(ammoCount);
    }
    private void RefillAmmo(int ammoToRefill)
    {
        ammoCount = Mathf.Clamp(ammoCount + ammoToRefill, 0, maxAmmo);
        OnAmmoRefill?.Invoke(ammoCount);
    }
    #endregion

    private void ActivateInvincibility()
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        invincibilityCoroutine = StartCoroutine(InvincibilityDuration());
    }
    private void StopInvincibility()
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = null;

            shipCollider.enabled = true;
        }
    }
    private IEnumerator InvincibilityDuration()
    {
        shipAnim.SetTrigger(shipAnim_InvincibilityHash);
        shipCollider.enabled = false;

        yield return invincibilityDuration;

        shipCollider.enabled = true;
        invincibilityCoroutine = null;
    }


    public void TakeDamage(int damageToTake)
    {
        if (shieldActive)
        {
            ShieldTakeDamage(damageToTake);
            return;
        }
        if (invincible) return;

        EntityHealth -= damageToTake;
        EntityHealth = Mathf.Clamp(EntityHealth, 0, EntityMaxHealth);
        ActivateInvincibility();

        if (EntityHealth == 0)
        {
            Death();
        }
        else
        {
            leftEngineAnim.SetBool(engineAnim_Hurt, EntityHealth < 3);
            rightEngineAnim.SetBool(engineAnim_Hurt, EntityHealth < 2);
        }

        AudioManager.Instance?.PlayOneShotClip(explosionClip);
        OnEntityDamaged?.Invoke(EntityHealth);
    }
    public void Death()
    {
        OnEntityKilled?.Invoke(this);

        StopPowerUp(PowerUp.Type.TripleShot);
        StopPowerUp(PowerUp.Type.Speed);
        StopPowerUp(PowerUp.Type.Shield);
        StopInvincibility();

        Instantiate(explosion, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void ShieldTakeDamage(int damageToTake)
    {
        shieldHealth -= damageToTake;
        shieldHealth = Mathf.Clamp(shieldHealth, 0, shieldMaxHealth);

        shieldAnim.SetInteger(shieldAnim_HealthHash, shieldHealth);

        if (shieldHealth == 0)
        {
            StopPowerUp(PowerUp.Type.Shield);
        }
        ActivateInvincibility();
    }
    #endregion
}