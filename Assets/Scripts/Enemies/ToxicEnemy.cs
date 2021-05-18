using UnityEngine;

public class ToxicEnemy : EnemyShooterBase
{
    #region Variables
    #region Components
    [Header("Components")]
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private Animator toxicShieldAnim;
    [SerializeField] private Collider2D enemyCol;
    [SerializeField] private AudioSource enemyAudioSource;
    #endregion

    #region Animator
    [Header("Animator References")]
    // Ship Animations
    [SerializeField] private string enemyAnim_DeathTrigger;
    private int enemyAnim_DeathTriggerHash;

    // Shield Animations
    [SerializeField] private string shieldAnim_HealthInt;
    private int shieldAnim_HealthIntHash;
    #endregion


    [Header("Enemy Move")]
    [SerializeField] private float enemySpeed = 6f;
    [SerializeField] private float shieldedSpeed = 4f;
    [SerializeField] [Range(0f, 1f)] private float enemyExplosionSpeedReduction = 0.5f;

    [Header("Enemy Shoot")]
    [SerializeField] private Transform laserLeftSpawnOffset;
    [SerializeField] private Transform laserRightSpawnOffset;
    [SerializeField] private Vector2 fireRate = new Vector2(2f, 4f);

    private float fireRateTimer = 0f;


    [Header("Toxic Shield")]
    private int shieldHealth = 0;
    private int shieldMaxHealth = 1;
    private bool shieldActive;


    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip explosionClip;
    #endregion


    #region MonoBehaviour Methods
    protected override void OnEnable()
    {
        base.OnEnable();

        enemyCol.enabled = true;
    }

    private void OnDisable()
    {
        ShieldRecharge();
    }

    protected void Start()
    {
        enemyAnim_DeathTriggerHash = Animator.StringToHash(enemyAnim_DeathTrigger);
        shieldAnim_HealthIntHash = Animator.StringToHash(shieldAnim_HealthInt);

        ShieldRecharge();
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        Shoot();
    }
    #endregion

    #region Custom Methods
    // I use this method to give a bit of anticipation to the player. The player should see the Enemy ship firing the lasers inside the screen.
    protected override void EnemyVisible()
    {
        // Reset fireRateTimer if the Enemy has entered again the screen
        fireRateTimer = Time.time + Random.Range(fireRate.x, fireRate.y) * Random.Range(0f, .4f);
    }

    // No longer visible. Has no functionality atm
    protected override void EnemyInvisible() { }

    // Enemy to Entity Collision
    protected override void EnemyCollide(Collider2D collision)
    {
        // If the enemy collided with a Damageable Entity
        if (collision.TryGetComponent(out IDamageable entityCollided))
        {
            // Deal Damage
            entityCollided.TakeDamage(collisionDamage);

            // Destroy Enemy
            TakeDamage(EntityMaxHealth);
        }
    }


    protected override void Move()
    {
        float selectedSpeed = (shieldActive) ? shieldedSpeed : enemySpeed;

        // Move downwards
        transform.Translate(Vector3.down * (selectedSpeed * (enemyCol.enabled ? 1f : enemyExplosionSpeedReduction)) * Time.deltaTime);

        // If the enemy is out of bounds, teleport it above the screen in a new X position
        if (Mathf.Abs(transform.position.y) > SpaceShooterData.EnemyBoundLimitsY || Mathf.Abs(transform.position.x) > SpaceShooterData.SpawnX)
        {
            if (EntityHealth > 0)
            {
                transform.position = new Vector3(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY, transform.position.z);

                if (transform.rotation.eulerAngles.z != 0f)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, (transform.position.x <= 0f) ? SpaceShooterData.MaxEnemyRotation : -SpaceShooterData.MaxEnemyRotation));
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    protected override void Shoot()
    {
        // If the GameManager is present in the scene...
        if (GameManager.Instance != null)
        {
            // ...and the Game has ended, stop Enemy firing lasers
            if (GameManager.Instance.IsGameOver()) return;
        }

        // If the Enemy is visible, and has health...
        if (isVisible && EntityHealth > 0f)
        {
            // If Time.time is higher than the Fire Rate Timer
            if (Time.time >= fireRateTimer)
            {
                // Update timer
                fireRateTimer = Time.time + Random.Range(fireRate.x, fireRate.y);

                // Play Shoot soundclip
                AudioManager.Instance?.PlayOneShotClip(shootClip);

                // Instantiate lasers from pool
                if (ObjectPool.Instance != null)
                {
                    GameObject laser = null;
                    laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserLeftSpawnOffset.position, laserLeftSpawnOffset.rotation);
                    laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserRightSpawnOffset.position, laserRightSpawnOffset.rotation);
                }
            }
        }
    }

    private void ShieldRecharge()
    {
        shieldActive = true;
        shieldHealth = shieldMaxHealth;
        toxicShieldAnim.SetInteger(shieldAnim_HealthIntHash, shieldHealth);
    }

    public override void TakeDamage(int damageToTake)
    {
        if (shieldActive)
        {
            // Take Shield Damage, and clamp the value
            shieldHealth = Mathf.Clamp(shieldHealth - damageToTake, 0, shieldMaxHealth);

            // Update Shield Animator
            toxicShieldAnim.SetInteger(shieldAnim_HealthIntHash, shieldHealth);

            if (shieldHealth == 0)
            {
                shieldActive = false;
                ObjectPool.Instance?.GetPooledObject(ObjectPool.PoolType.ToxicSmoke, transform.position, Quaternion.identity);
            }

            return;
        }

        base.TakeDamage(damageToTake);
    }

    // IDamageable - Behaviour to run when the Enemy has no health left
    public override void Death()
    {
        base.Death();
        Explode();
    }

    // Removes collisions, slows down movement, plays explosion animation. Once the animation triggers a event, DestroyEnemy will be called
    private void Explode()
    {
        enemyCol.enabled = false;

        enemyAnim.SetTrigger(enemyAnim_DeathTriggerHash);
        enemyAudioSource.PlayOneShot(explosionClip);
        AddScore();
    }
    #endregion
}