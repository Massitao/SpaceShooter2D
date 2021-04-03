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

    [Header("Score")]
    [SerializeField] private int scoreToGive = 5;
    #endregion

    #region Events
    // Events
    public System.Action<int> OnEntityDamaged { get; set; }
    public System.Action<IDamageable> OnEntityKilled { get; set; }
    #endregion
    #endregion


    #region MonoBehaviour Methods
    void Start()
    {
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