using System.Collections;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable
{
    #region Variables
    #region Components
    [Header("Components")]
    [SerializeField] private Collider2D     shipCollider;
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
    #endregion

    #region Ship Properties
    [Header("Ship Health")]
    [SerializeField] private int entityHealth;
    public int EntityHealth { get { return entityHealth; } set { entityHealth = value; } }

    [Header("Ship Move")]
    [SerializeField] private float shipSpeed = 3f;

    [Header("Ship Shoot")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Vector3 laserSpawnOffset;
    [SerializeField] private float fireRate = .5f;
    private float fireRateTimer;

    [SerializeField] private AudioClip shootClip;
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
    public System.Action<int> OnEntityDamaged { get; set; }
    public System.Action<IDamageable> OnEntityKilled { get; set; }
    #endregion
    #endregion


    #region MonoBehaviour Methods
    // Update is called once per frame
    private void Update()
    {
        UpdateInput();
        Move();
        Shoot();
    }
    #endregion

    #region Custom Methods
    private void UpdateInput()
    {
        // Get Player Input
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Update Ship Move Animations
        shipAnim.SetFloat(shipAnim_InputHash, moveInput.x);
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
        if (Input.GetKey(KeyCode.Space))
        {
            if (Time.time >= fireRateTimer)
            {
                // Update timer
                fireRateTimer = Time.time + fireRate;

                // Choose between Triple or Single laser
                GameObject laserType = tripleLaserActive ? tripleLaserPrefab : laserPrefab;

                // Ignore own lasers
                Collider2D[] lasersToIgnore = Instantiate(laserType, transform.position + laserSpawnOffset, Quaternion.identity).GetComponentsInChildren<Collider2D>();
                for (int i = 0; i < lasersToIgnore.Length; i++)
                {
                    Physics2D.IgnoreCollision(shipCollider, lasersToIgnore[i], true);
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

    public void TakeDamage(int damageToTake)
    {
        if (shieldActive)
        {
            StopPowerUp(PowerUp.Type.Shield);
            return;
        }

        EntityHealth -= damageToTake;
        EntityHealth = Mathf.Clamp(EntityHealth, 0, 100);
        OnEntityDamaged?.Invoke(EntityHealth);

        switch (entityHealth)
        {
            case 2:
                leftEngineAnim.SetTrigger(engineAnim_HurtHash);
                break;

            case 1:
                leftEngineAnim.SetTrigger(engineAnim_HurtHash);
                rightEngineAnim.SetTrigger(engineAnim_HurtHash);
                break;
        }

        if (EntityHealth == 0)
        {
            Death();
        }
    }
    public void Death()
    {
        OnEntityKilled?.Invoke(this);

        StopPowerUp(PowerUp.Type.TripleShot);
        StopPowerUp(PowerUp.Type.Speed);
        StopPowerUp(PowerUp.Type.Shield);

        Destroy(gameObject);
    }
    #endregion
}