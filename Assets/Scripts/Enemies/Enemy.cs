using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    #region Variables
    #region Components
    [Header("Components")]
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private Collider2D enemyCol;
    [SerializeField] private AudioSource enemyAudioSource;
    #endregion

    #region Animator
    [Header("Animator References")]
    [SerializeField] private string enemyAnim_DeathTrigger;
    private int enemyAnim_DeathTriggerHash => Animator.StringToHash(enemyAnim_DeathTrigger);
    #endregion

    #region Enemy Properties
    [Header("Enemy Health")]
    [SerializeField] private int entityHealth;
    public int EntityHealth
    {
        get { return entityHealth; }
        set { entityHealth = Mathf.Clamp(value, 0, EntityMaxHealth); }
    }

    [SerializeField] private int entityMaxHealth;
    public int EntityMaxHealth
    {
        get { return entityMaxHealth; }
        set { entityMaxHealth = Mathf.Clamp(value, 1, int.MaxValue); }
    }

    [Header("Enemy Move")]
    [SerializeField] private float enemySpeed = 4f;


    [Header("Enemy Shoot")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Vector3 laserSpawnOffset;
    [SerializeField] private Vector2 fireRate = new Vector2(2f, 4f);

    [SerializeField] [Range(0, 30)] private int enemyProjectileLayer;
    [SerializeField] private bool isVisible = false;

    private float fireRateTimer = 0f;


    [Header("Enemy On Collision Damage")]
    [SerializeField] private int collisionDamage = 1;


    [Header("Score")]
    [SerializeField] private int scoreToGive;


    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip explosionClip;
    #endregion

    #region Events
    // Events
    public event System.Action<int> OnEntityDamaged;
    public event System.Action<IDamageable> OnEntityKilled;
    #endregion
    #endregion


    #region MonoBehaviour Methods
    private void Start()
    {
        // Set Enemy Health to Max Health
        entityHealth = entityMaxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        Shoot();
    }

    // I use this method to give a bit of anticipation to the player. The player should see the Enemy ship firing the lasers inside the screen
    private void OnBecameVisible()
    {
        isVisible = true;

        // Reset fireRateTimer if the Enemy has entered again the screen
        fireRateTimer = Time.time + Random.Range(fireRate.x, fireRate.y) * Random.Range(0f, .4f);
    }
    // No longer visible
    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
    #endregion

    #region Custom Methods
    private void Move()
    {
        // Move downwards
        transform.Translate(Vector3.down * enemySpeed * Time.deltaTime);

        // If the enemy is out of bounds, teleport it above the screen in a new X position
        if (transform.position.y <= SpaceShooterData.EnemyBoundLimitsY.x)
        {
            transform.position = new Vector3(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY.y, transform.position.z);
        }
    }
    private void Shoot()
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

                // Ignore own lasers
                Transform[] lasersToIgnore = Instantiate(laserPrefab, transform.position + laserSpawnOffset, Quaternion.identity).GetComponentsInChildren<Transform>();
                for (int i = 0; i < lasersToIgnore.Length; i++)
                {
                    // Set Enemy Projectile Laser
                    lasersToIgnore[i].gameObject.layer = enemyProjectileLayer;
                }

                // Play Shoot soundclip
                AudioManager.Instance?.PlayOneShotClip(shootClip);
            }
        }
    }

    // IDamageable - Behaviour to run when the Asteroid is damaged
    public void TakeDamage(int damageToTake)
    {
        // Subtract health
        EntityHealth = Mathf.Clamp(EntityHealth - damageToTake, 0, EntityMaxHealth);

        // Invoke event, passing in the "EntityHealth" value
        OnEntityDamaged?.Invoke(EntityHealth);

        // If the Enemy has no more health left, trigger Explode
        if (EntityHealth == 0)
        {
            Explode();
        }
    }

    // Removes collisions, slows down movement, plays explosion animation
    private void Explode()
    {
        enemyCol.enabled = false;
        enemySpeed = 2;

        enemyAnim.SetTrigger(enemyAnim_DeathTriggerHash);
        enemyAudioSource.PlayOneShot(explosionClip);
        GameManager.Instance?.AddScore(scoreToGive);
    }

    // IDamageable - Behaviour to run when the Enemy has no health left
    public void Death()
    {
        OnEntityKilled?.Invoke(this);
        Destroy(gameObject);
    }
    #endregion
}