using UnityEngine;

public class RookieEnemy : EnemyShooterBase
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
    private int enemyAnim_DeathTriggerHash;
    #endregion


    [Header("Enemy Move")]
    [SerializeField] private float enemySpeed = 4f;
    [SerializeField] private float enemyExplosionSpeed = 2f;

    [Header("ZigZag Move")]
    [SerializeField] private float zigZagSpeedX;
    [SerializeField] private float zigZagSpeedY;
    [SerializeField] private float zigZagMaxMove;
    [SerializeField] [Range(0f, 1f)] private float zigZagSelectionPercentage;

    private float startPosX;
    private bool moveZigZag;
    private bool movingRight;


    [Header("Enemy Shoot")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Vector2 laserLeftSpawnOffset;
    [SerializeField] private Vector2 laserRightSpawnOffset;
    [SerializeField] private Vector2 fireRate = new Vector2(2f, 4f);

    [SerializeField] [Range(0, 30)] private int enemyProjectileLayer;

    private float fireRateTimer = 0f;


    [Header("Enemy On Collision Damage")]
    [SerializeField] protected int collisionDamage = 1;


    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip explosionClip;
    #endregion


    #region MonoBehaviour Methods
    protected override void Start()
    {
        base.Start();

        startPosX = transform.position.x;
        if (Random.value < zigZagSelectionPercentage) moveZigZag = true;

        enemyAnim_DeathTriggerHash = Animator.StringToHash(enemyAnim_DeathTrigger);
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
        if (moveZigZag) ZigZagMove();
        else NormalMove();

        // If the enemy is out of bounds, teleport it above the screen in a new X position
        if (transform.position.y <= SpaceShooterData.EnemyBoundLimitsY.x)
        {
            transform.position = new Vector3(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY.y, transform.position.z);
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

                // Ignore own lasers
                Instantiate(laserPrefab, transform.position + (Vector3)laserLeftSpawnOffset, Quaternion.identity).transform.gameObject.layer = enemyProjectileLayer;
                Instantiate(laserPrefab, transform.position + (Vector3)laserRightSpawnOffset, Quaternion.identity).transform.gameObject.layer = enemyProjectileLayer;

                // Play Shoot soundclip
                AudioManager.Instance?.PlayOneShotClip(shootClip);
            }
        }
    }

    private void NormalMove()
    {
        // Move downwards
        transform.Translate(Vector3.down * enemySpeed * Time.deltaTime);
    }
    private void ZigZagMove()
    {
        // Move in ZigZag
        if (transform.position.x >= startPosX + zigZagMaxMove) movingRight = false;
        if (transform.position.x <= startPosX - zigZagMaxMove) movingRight = true;

        Vector2 dirToMove = (movingRight ? Vector2.down + Vector2.right : Vector2.down + Vector2.left).normalized;
        dirToMove.x *= zigZagSpeedX;
        dirToMove.y *= zigZagSpeedY;

        transform.Translate(dirToMove * Time.deltaTime);
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
        enemySpeed = enemyExplosionSpeed;

        enemyAnim.SetTrigger(enemyAnim_DeathTriggerHash);
        enemyAudioSource.PlayOneShot(explosionClip);
        AddScore();
    }
    #endregion
}