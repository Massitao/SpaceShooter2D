using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessEnemy : EnemyShooterBase
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
    [SerializeField] private float recklessRamSpeed = 6f;
    [SerializeField] private float recklessAngularSpeed = 5f;
    [SerializeField] [Range(0f, 1f)] private float enemyExplosionSpeedReduction = 0.5f;

    [Header("Enemy Shoot")]
    [SerializeField] private Transform laserSpawnOffset;
    [SerializeField] private Vector2 fireRate = new Vector2(2f, 4f);

    private float fireRateTimer = 0f;


    [Header("Reckless Checker")]
    [SerializeField] private float recklessCheckUpdate;
    [SerializeField] private float recklessCheckRadius;
    [SerializeField] private LayerMask recklessCheckLM;
    private Transform targetToFollow;
    private Collider2D[] recklessCheckerEntitiesDetected = new Collider2D[1];
    private Coroutine recklessCheckCoroutine;


    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip explosionClip;
    #endregion


    #region MonoBehaviour Methods
    protected override void OnEnable()
    {
        base.OnEnable();

        enemyCol.enabled = true;
        StartRecklessChecker();
    }
    private void OnDisable()
    {
        StopRecklessChecker();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, recklessCheckRadius);
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
        float selectedSpeed = enemySpeed;

        // If the Missile has a target, get a direction and rotate the GameObject towards the target
        if (targetToFollow != null && EntityHealth != 0)
        {
            Vector3 enemyTargetDir = (targetToFollow.transform.position - transform.position).normalized;
            transform.Rotate(0f, 0f, Vector3.Cross(enemyTargetDir, transform.up).z * recklessAngularSpeed * Time.deltaTime);

            selectedSpeed = recklessRamSpeed;
        }


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
        if (isVisible && EntityHealth > 0f && !targetToFollow)
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
                    laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserSpawnOffset.position, transform.rotation);
                }
            }
        }
    }


    private void StartRecklessChecker()
    {
        if (recklessCheckCoroutine != null)
        {
            StopCoroutine(recklessCheckCoroutine);
        }

        recklessCheckCoroutine = StartCoroutine(RecklessCheckerCoroutine());
    }
    private IEnumerator RecklessCheckerCoroutine()
    {
        int shipDetected = 0;

        while (true)
        {
            shipDetected = Physics2D.OverlapCircleNonAlloc(transform.position, recklessCheckRadius, recklessCheckerEntitiesDetected, recklessCheckLM);

            if (shipDetected > 0)
            {
                if (recklessCheckerEntitiesDetected[0].TryGetComponent(out Ship ship))
                {
                    targetToFollow = ship.transform;
                }
            }
            else
            {
                targetToFollow = null;
            }

            yield return new WaitForSeconds(recklessCheckUpdate);
        }
    }
    private void StopRecklessChecker()
    {
        if (recklessCheckCoroutine != null)
        {
            StopCoroutine(recklessCheckCoroutine);
            recklessCheckCoroutine = null;
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