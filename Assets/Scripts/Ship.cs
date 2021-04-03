using System.Collections;
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

    // ENGINES
    [SerializeField] private string engineAnim_Hurt;
    private int engineAnim_HurtHash => Animator.StringToHash(engineAnim_Hurt);

    // SHIELD
    [SerializeField] private string shieldAnim_Active;
    private int shieldAnimActiveHash => Animator.StringToHash(shieldAnim_Active);
    #endregion

    #region Input Values
    [Header("Inputs")]
    private Vector2 moveInput;
    private bool doShoot;
    #endregion

    #region Ship Properties
    [Header("Ship Health")]
    [SerializeField] private int entityHealth;
    public int EntityHealth { get { return entityHealth; } set { entityHealth = value; } }

    [Header("Ship Explosion")]
    [SerializeField] private GameObject explosion;

    [Header("Ship Move")]
    [SerializeField] private float shipSpeed = 3f;

    [Header("Ship Shoot")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Vector3 laserSpawnOffset;
    [SerializeField] private float fireRate = .5f;
    private float fireRateTimer;

    [Header("Ship Invincibility")]
    private WaitForSeconds invincibilityDuration = new WaitForSeconds(2f);
    private Coroutine invincibilityCoroutine;
    private bool invincible => invincibilityCoroutine != null;

    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip explosionClip;
    #endregion

    #region PowerUps
    [Header("Abilities")]
    private WaitForSeconds abilityDuration = new WaitForSeconds(5f);

    [Header("Triple Shoot")]
    [SerializeField] private GameObject tripleLaserPrefab;
    private Coroutine tripleShotCoroutine;
    private bool tripleLaserActive => tripleShotCoroutine != null;

    [Header("Extra Speed")]
    [SerializeField] [Range(1f, 3f)] private float extraSpeedMultiplier;
    private Coroutine extraSpeedCoroutine;
    private bool extraSpeedActive => extraSpeedCoroutine != null;

    [Header("Shield")]
    private Coroutine shieldCoroutine;
    private bool shieldActive => shieldCoroutine != null;
    #endregion

    #region Events
    // Events
    public event System.Action<int> OnEntityDamaged;
    public event System.Action<IDamageable> OnEntityKilled;
    #endregion
    #endregion


    #region MonoBehaviour Methods
    // Update is called once per frame
    private void Update()
    {
        Move();
        Shoot();
    }
    #endregion

    #region Custom Methods
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
    }

    private void Move()
    {
        // If Extra Speed Power-up is active, apply extra speed. Else, keep normal ship speed
        float speedToApply = extraSpeedActive ? shipSpeed * extraSpeedMultiplier : shipSpeed;
        transform.Translate(moveInput * speedToApply * Time.deltaTime);

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
        if (doShoot)
        {
            if (Time.time >= fireRateTimer)
            {
                // Update timer
                fireRateTimer = Time.time + fireRate;

                // Choose between Triple or Single laser
                GameObject laserType = tripleLaserActive ? tripleLaserPrefab : laserPrefab;

                // Ignore own lasers
                Transform[] lasersToIgnore = Instantiate(laserType, transform.position + laserSpawnOffset, Quaternion.identity).GetComponentsInChildren<Transform>();
                for (int i = 0; i < lasersToIgnore.Length; i++)
                {
                    lasersToIgnore[i].gameObject.layer = gameObject.layer;
                }

                // Play Shoot soundclip
                AudioManager.Instance?.PlayOneShotClip(shootClip);
            }
        }
    }

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
                if (tripleShotCoroutine != null)
                {
                    StopCoroutine(tripleShotCoroutine);
                }

                tripleShotCoroutine = StartCoroutine(TripleShotDuration());
                break;

            case PowerUp.Type.Speed:
                if (extraSpeedCoroutine != null)
                {
                    StopCoroutine(extraSpeedCoroutine);
                }

                extraSpeedCoroutine = StartCoroutine(ExtraSpeedDuration());
                break;

            case PowerUp.Type.Shield:
                if (shieldCoroutine != null)
                {
                    StopCoroutine(shieldCoroutine);
                }

                shieldCoroutine = StartCoroutine(ShieldDuration());
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

            case PowerUp.Type.Speed:
                if (extraSpeedCoroutine != null)
                {
                    StopCoroutine(extraSpeedCoroutine);
                    extraSpeedCoroutine = null;
                }
                break;

            case PowerUp.Type.Shield:
                if (shieldCoroutine != null)
                {
                    StopCoroutine(shieldCoroutine);
                    shieldAnim.SetBool(shieldAnimActiveHash, false);
                    shieldCoroutine = null;
                }
                break;
        }
    }

    private IEnumerator TripleShotDuration()
    {
        yield return abilityDuration;
        tripleShotCoroutine = null;
    }
    private IEnumerator ExtraSpeedDuration()
    {
        yield return abilityDuration;
        extraSpeedCoroutine = null;
    }
    private IEnumerator ShieldDuration()
    {
        shieldAnim.SetBool(shieldAnimActiveHash, true);
    
        yield return abilityDuration;

        shieldAnim.SetBool(shieldAnimActiveHash, false);

        shieldCoroutine = null;
    }

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
        }
    }
    private IEnumerator InvincibilityDuration()
    {
        yield return invincibilityDuration;
        invincibilityCoroutine = null;
    }

    public void TakeDamage(int damageToTake)
    {
        if (shieldActive)
        {
            StopPowerUp(PowerUp.Type.Shield);
            ActivateInvincibility();
            return;
        }

        if (invincible) return;

        EntityHealth -= damageToTake;
        EntityHealth = Mathf.Clamp(EntityHealth, 0, 100);
        ActivateInvincibility();

        OnEntityDamaged?.Invoke(EntityHealth);

        switch (entityHealth)
        {
            case 2:
                leftEngineAnim.SetTrigger(engineAnim_HurtHash);
                AudioManager.Instance?.PlayOneShotClip(explosionClip);
                break;

            case 1:
                leftEngineAnim.SetTrigger(engineAnim_HurtHash);
                rightEngineAnim.SetTrigger(engineAnim_HurtHash);
                AudioManager.Instance?.PlayOneShotClip(explosionClip);
                break;

            case 0:
                Death();
                break;
        }
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
    #endregion
}