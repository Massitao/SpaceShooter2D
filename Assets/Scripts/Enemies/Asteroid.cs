using UnityEngine;

public class Asteroid : MonoBehaviour, IDamageable
{
    #region Variables
    #region Asteroid Properties
    [Header("Asteroid Properties")]
    [SerializeField] private float asteroidMoveSpeed = 2f;
    [SerializeField] private float asteroidRotateAnglePerSecond = 5f;
    [SerializeField] private GameObject explosionPrefab;
    private bool inverseSpin;

    [Header("Asteroid On Collision Damage")]
    [SerializeField] private int collisionDamage = 3;

    [Header("Asteroid Health")]
    [HideInInspector] private int entityHealth;
    public int EntityHealth
    {
        get { return entityHealth; }
        set { entityHealth = Mathf.Clamp(value, 0, EntityMaxHealth); }
    }

    [HideInInspector] private int entityMaxHealth = 1;
    public int EntityMaxHealth
    {
        get { return entityMaxHealth; }
        set { entityMaxHealth = Mathf.Clamp(value, 1, int.MaxValue); }
    }


    [Header("Score")]
    [SerializeField] private int scoreToGive = 5;
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
        // Setting Entity Health to Max Health
        entityHealth = entityMaxHealth;

        // Setting random rotation direction
        inverseSpin = Random.value >= .5f;
        asteroidRotateAnglePerSecond *= inverseSpin ? -1f : 1f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the asteroid collided with a Damageable Entity
        if (collision.TryGetComponent(out IDamageable entity))
        {
            // Deal damage
            entity.TakeDamage(collisionDamage);

            // Destroy Asteroid
            TakeDamage(EntityMaxHealth);
        }
    }
    #endregion

    #region Custom Methods
    // Move downwards using world axis
    private void Move()
    {
        transform.Translate(Vector3.down * asteroidMoveSpeed * Time.deltaTime, Space.World);
    }

    // Rotate GameObject using the Z axis
    private void Rotate()
    {
        transform.Rotate(Vector3.forward * asteroidRotateAnglePerSecond * Time.deltaTime);
    }

    // IDamageable - Behaviour to run when the Asteroid is damaged.
    public void TakeDamage(int damageToTake)
    {
        // Invokes event, passing in the "damageToTake" value
        OnEntityDamaged?.Invoke(damageToTake);

        Death();
    }

    // IDamageable - Behaviour to run when the Asteroid has no health left
    public void Death()
    {
        // If the GameManager is active in the scene, add score
        GameManager.Instance?.AddScore(scoreToGive);

        // Invokes event, passing in this Entity reference
        OnEntityKilled?.Invoke(this);
        
        // Instantiate explosion and destroy this GameObject after 0.1f seconds
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, .1f);
    }
    #endregion
}