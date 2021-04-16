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

    [HideInInspector] private int entityHealth;
    public int EntityHealth { get { return entityHealth; } set { entityHealth = value; } }

    [HideInInspector] private int entityMaxHealth = 1;
    public int EntityMaxHealth { get { return entityMaxHealth; } set { entityMaxHealth = value; } }

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
        entityHealth = entityMaxHealth;

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
        if (collision.TryGetComponent(out Ship player))
        {
            // Kill player
            player.TakeDamage(4);
            Death();
        }
    }
    #endregion

    #region Custom Methods
    private void Move()
    {
        transform.Translate(Vector3.down * asteroidMoveSpeed * Time.deltaTime, Space.World);
    }
    private void Rotate()
    {
        transform.Rotate(Vector3.forward * asteroidRotateAnglePerSecond * Time.deltaTime);
    }

    public void TakeDamage(int damageToTake)
    {
        OnEntityDamaged?.Invoke(0);
        Death();
    }
    public void Death()
    {
        GameManager.Instance?.AddScore(scoreToGive);
        OnEntityKilled?.Invoke(this);
        
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 0.1f);
    }
    #endregion
}