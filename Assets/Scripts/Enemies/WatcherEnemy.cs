using System.Collections;
using UnityEngine;

public class WatcherEnemy : EnemyShooterBase
{
    #region Variables
    #region Components
    [Header("Components")]
    [SerializeField] private Collider2D enemyCol;
    [SerializeField] private AudioSource enemyAudioSource;
    #endregion

    [Header("Enemy Move")]
    [SerializeField] private float enemySpeed = 4f;
    [SerializeField] [Range(0f, 1f)] private float enemyExplosionSpeedReduction = 0.5f;

    [Header("Enemy Shoot")]
    [SerializeField] private Transform laserDownSpawnOffset;
    [SerializeField] private Transform laserLeftSpawnOffset;
    [SerializeField] private Transform laserRightSpawnOffset;
    [SerializeField] private Transform laserUpLeftSpawnOffset;
    [SerializeField] private Transform laserUpRightSpawnOffset;
    [SerializeField] private Vector2 fireRate = new Vector2(2f, 4f);

    private float fireRateTimer = 0f;


    [Header("Watcher Properties")]
    [SerializeField] private float watcherCheckUpdate;
    [SerializeField] private LayerMask watcherCheckLM;
    private Transform targetToShoot;
    private float targetToWatcherAngle;
    private Coroutine watcherCheckerCoroutine;


    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip explosionClip;
    #endregion


    #region MonoBehaviour Methods
    protected override void OnEnable()
    {
        base.OnEnable();

        enemyCol.enabled = true;
        StartWatcherChecker();
    }
    private void OnDisable()
    {
        StopWatcherChecker();
    }


    // Update is called once per frame
    private void Update()
    {
        Move();
        UpdateTargetAngle();
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
        // Move downwards
        transform.Translate(Vector3.down * (enemySpeed * (enemyCol.enabled ? 1f : enemyExplosionSpeedReduction)) * Time.deltaTime);

        // If the enemy is out of bounds, teleport it above the screen in a new X position
        if (Mathf.Abs(transform.position.y) > SpaceShooterData.EnemyBoundLimitsY || Mathf.Abs(transform.position.x) > SpaceShooterData.WrapX)
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

                    // Down Cannon
                    if ((targetToWatcherAngle >= -45f && targetToWatcherAngle <= 45f) || targetToShoot == null)
                    {
                        laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserDownSpawnOffset.position, laserDownSpawnOffset.rotation);
                    }
                    // Left Cannon
                    else if (targetToWatcherAngle > 45f && targetToWatcherAngle <= 135f)
                    {
                        laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserLeftSpawnOffset.position, laserLeftSpawnOffset.rotation);
                    }
                    // Right Cannon
                    else if (targetToWatcherAngle < -45f && targetToWatcherAngle >= -135f)
                    {
                        laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserRightSpawnOffset.position, laserRightSpawnOffset.rotation);
                    }
                    // Up Cannons
                    else
                    {
                        laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserUpLeftSpawnOffset.position, laserUpLeftSpawnOffset.rotation);
                        laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserUpRightSpawnOffset.position, laserUpRightSpawnOffset.rotation);
                    }
                }
            }
        }
    }


    private void StartWatcherChecker()
    {
        if (watcherCheckerCoroutine != null)
        {
            StopCoroutine(watcherCheckerCoroutine);
        }

        watcherCheckerCoroutine = StartCoroutine(WatcherCheckerCoroutine());
    }
    private IEnumerator WatcherCheckerCoroutine()
    {
        Collider2D target = null;

        while (true)
        {
            target = Physics2D.OverlapCircle(transform.position, 100f, watcherCheckLM);

            if (target != null && target.TryGetComponent(out Ship ship))
            {
                targetToShoot = ship.transform;
            }
            else
            {
                targetToShoot = null;
            }

            yield return new WaitForSeconds(watcherCheckUpdate);
        }
    }
    private void StopWatcherChecker()
    {
        if (watcherCheckerCoroutine != null)
        {
            StopCoroutine(watcherCheckerCoroutine);
            watcherCheckerCoroutine = null;
        }
    }

    private void UpdateTargetAngle()
    {
        if (targetToShoot == null) return;

        Vector3 dir = targetToShoot.position - transform.position;
        targetToWatcherAngle = Vector3.SignedAngle(dir, -transform.up, Vector3.forward);
    }


    // IDamageable - Behaviour to run when the Enemy has no health left
    public override void Death()
    {
        enemyCol.enabled = false;

        // Instantiate explosion and disable this GameObject after 0.1f seconds
        GameObject explosion = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Explosion, transform.position, Quaternion.identity);
        base.Death();

        DisableEnemy(.2f);
    }
    #endregion
}