using System.Collections;
using UnityEngine;

public class AvoiderEnemy : EnemyShooterBase
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
    [SerializeField] private float avoidSpeed = 8f;
    [SerializeField] [Range(0f, 1f)] private float enemyExplosionSpeedReduction = 0.5f;

    [Header("Enemy Shoot")]
    [SerializeField] private Transform laserLeftSpawnOffset;
    [SerializeField] private Transform laserRightSpawnOffset;
    [SerializeField] private Vector2 fireRate = new Vector2(2f, 4f);

    private float fireRateTimer = 0f;


    [Header("Avoider Properties")]
    [SerializeField] private float avoiderCheckRadius;
    [SerializeField] private float avoiderCheckRadiusThreshold;
    [SerializeField] private float avoiderCheckUpdate;
    [SerializeField] private LayerMask avoiderCheckLM;
    private Collider2D[] avoiderCollisions = new Collider2D[6];
    private Vector2 avoidDir;
    private Coroutine avoiderCheckerCoroutine;


    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip explosionClip;
    #endregion


    #region MonoBehaviour Methods
    protected override void OnEnable()
    {
        base.OnEnable();

        enemyCol.enabled = true;
        StartAvoiderChecker();
    }

    private void OnDisable()
    {
        StopAvoiderChecker();
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
        Gizmos.DrawWireSphere(transform.position, avoiderCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoiderCheckRadiusThreshold);
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
        Vector2 dirToMove = (Vector2.down + avoidDir).normalized;
        float selectedSpeed = (avoidDir != Vector2.zero) ? avoidSpeed : enemySpeed;
        selectedSpeed *= (enemyCol.enabled ? 1f : enemyExplosionSpeedReduction);

        transform.Translate(dirToMove * selectedSpeed * Time.deltaTime);

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
                    laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserLeftSpawnOffset.position, transform.rotation);
                    laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, laserRightSpawnOffset.position, transform.rotation);
                }
            }
        }
    }


    private void StartAvoiderChecker()
    {
        if (avoiderCheckerCoroutine != null)
        {
            StopCoroutine(avoiderCheckerCoroutine);
        }

        avoiderCheckerCoroutine = StartCoroutine(AvoiderCheckerCoroutine());
    }
    private IEnumerator AvoiderCheckerCoroutine()
    {
        int lasersDetected = 0;
        int avoidLeft = 0;
        float distance = 0f;

        while (true)
        {
            avoidLeft = 0;
            lasersDetected = Physics2D.OverlapCircleNonAlloc(transform.position, avoiderCheckRadius, avoiderCollisions, avoiderCheckLM);

            if (lasersDetected != 0)
            {
                for (int laserIndex = 0; laserIndex < lasersDetected; laserIndex++)
                {
                    // If Collider detected is LaserBeam, just avoid it at all costs, ignoring other lasers
                    if (avoiderCollisions[laserIndex].TryGetComponent(out LaserBeam beam))
                    {
                        avoidLeft = (int)Mathf.Sign(transform.position.x - avoiderCollisions[laserIndex].transform.position.x);
                        break;
                    }

                    // Checks if the player lasers are coming towards the ship and if the distance reaches the threshold. If true, add to the avoidance direction
                    if (avoiderCollisions[laserIndex].transform.position.y < transform.position.y)
                    {
                        distance = Vector3.Distance(transform.position, avoiderCollisions[laserIndex].transform.position);

                        if (distance <= avoiderCheckRadiusThreshold)
                        {
                            avoidLeft += (int)Mathf.Sign(transform.position.x - avoiderCollisions[laserIndex].transform.position.x);
                        }
                    }
                }

                avoidDir = Vector2.right * Mathf.Clamp(avoidLeft, -1, 1);
            }
            else
            {
                avoidDir = Vector2.zero;
            }

            yield return new WaitForSeconds(avoiderCheckUpdate);
        }
    }
    private void StopAvoiderChecker()
    {
        if (avoiderCheckerCoroutine != null)
        {
            StopCoroutine(avoiderCheckerCoroutine);
            avoidDir = Vector2.zero;
            avoiderCheckerCoroutine = null;
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
        StopAvoiderChecker();

        enemyAnim.SetTrigger(enemyAnim_DeathTriggerHash);
        enemyAudioSource.PlayOneShot(explosionClip);
        AddScore();
    }
    #endregion
}