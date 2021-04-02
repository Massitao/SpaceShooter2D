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
    [Header("Enemy Properties")]
    [SerializeField] private int entityHealth;
    public int EntityHealth { get { return entityHealth; } set { entityHealth = value; } }

    [SerializeField] private float enemySpeed = 4f;

    [Header("Score")]
    [SerializeField] private int scoreToGive;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionClip;
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
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable entityCollided))
        {
            entityCollided.TakeDamage(1);
            Explode();
        }
    }
    #endregion

    #region Custom Methods
    private void Move()
    {
        // Move downwards
        transform.Translate(Vector3.down * enemySpeed * Time.deltaTime);

        // If the enemy is out of bounds, tp it above the screen in a new X position
        if (transform.position.y <= SpaceShooterData.EnemyBoundLimitsY.x)
        {
            transform.position = new Vector3(Random.Range(-SpaceShooterData.EnemySpawnX, SpaceShooterData.EnemySpawnX), SpaceShooterData.EnemyBoundLimitsY.y, transform.position.z);
        }
    }

    public void TakeDamage(int damageToTake)
    {
        EntityHealth -= damageToTake;
        EntityHealth = Mathf.Clamp(EntityHealth, 0, 100);
        OnEntityDamaged?.Invoke(EntityHealth);

        if (entityHealth == 0)
        {
            Explode();
        }
    }
    private void Explode()
    {
        enemyCol.enabled = false;
        enemySpeed = 2;

        enemyAnim.SetTrigger(enemyAnim_DeathTriggerHash);
        enemyAudioSource.PlayOneShot(explosionClip);
    }
    public void Death()
    {
        GameManager.Instance?.AddScore(scoreToGive);
        OnEntityKilled?.Invoke(this);
        Destroy(gameObject);
    }
    #endregion
}