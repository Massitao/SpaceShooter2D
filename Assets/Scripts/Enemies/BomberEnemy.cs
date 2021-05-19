using UnityEngine;

public class BomberEnemy : EnemyShooterBase
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
    [SerializeField] [Range(0f, 1f)] private float enemyExplosionSpeedReduction = 0.5f;
    [SerializeField] private float sinFrequency = 4f;
    [SerializeField] private float sinAmplitude = 4f;

    Vector2 dirToMove = Vector2.zero;


    [Header("Enemy Shoot")]
    [SerializeField] private Transform bombSpawn;

    private float bombFirePos;
    private bool bombFiredInScreen;


    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip explosionClip;
    #endregion


    #region MonoBehaviour Methods
    protected override void OnEnable()
    {
        base.OnEnable();

        enemyCol.enabled = true;
        bombFiredInScreen = false;

        bombFirePos = Random.Range(-SpaceShooterData.EnemyBoundLimitsY * 0.25f, SpaceShooterData.EnemyBoundLimitsY * 0.8f);
    }

    protected void Start()
    {
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
    // Visible. Has no functionality atm
    protected override void EnemyVisible() { }

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
        if (EntityHealth > 0)
        {
            dirToMove = (Vector2.right + Vector2.down).normalized;
            dirToMove.x *= Mathf.Sin(Time.time * sinFrequency) * sinAmplitude;
            dirToMove.y *= (enemySpeed * (enemyCol.enabled ? 1f : enemyExplosionSpeedReduction));
        }
        else
        {
            dirToMove = dirToMove.normalized * enemySpeed * enemyExplosionSpeedReduction;
        }

        // Move downwards
        transform.Translate(dirToMove * Time.deltaTime);

        // If the enemy is out of bounds, teleport it above the screen in a new X position
        if (Mathf.Abs(transform.position.y) > SpaceShooterData.EnemyBoundLimitsY || Mathf.Abs(transform.position.x) > SpaceShooterData.WrapX)
        {
            if (EntityHealth > 0)
            {
                transform.position = new Vector3(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY, transform.position.z);
                bombFirePos = Random.Range(-SpaceShooterData.EnemyBoundLimitsY * 0.25f, SpaceShooterData.EnemyBoundLimitsY * 0.8f);
                bombFiredInScreen = false;
            }
            else
            {
                DisableEnemy();
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

        // If the Enemy hasn't dropped a bomb, and has health...
        if (!bombFiredInScreen && EntityHealth > 0f)
        {
            // If Time.time is higher than the Fire Rate Timer
            if (Mathf.Abs(bombFirePos - transform.position.y) <= 0.1f)
            {
                // Block bombing
                bombFiredInScreen = true;

                // Play Shoot soundclip
                AudioManager.Instance?.PlayOneShotClip(shootClip);

                // Instantiate lasers from pool
                if (ObjectPool.Instance != null)
                {
                    GameObject bomb = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Bomb, bombSpawn.position, transform.rotation);
                }
            }
        }
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