using UnityEngine;

public class Asteroid : EnemyBase
{
    #region Variables
    [Header("Asteroid Properties")]
    [SerializeField] private float asteroidMoveSpeed = 2f;
    [SerializeField] private float asteroidRotateAnglePerSecond = 5f;
    private bool inverseSpin;


    [Header("Asteroid On Collision Damage")]
    [SerializeField] private int collisionDamage = 3;


    [Header("Asteroid Explosion")]
    [SerializeField] private float asteroidDestroyDelay;
    #endregion


    #region MonoBehaviour Methods
    protected override void OnEnable()
    {
        base.OnEnable();

        // Setting random rotation direction
        inverseSpin = Random.value >= .5f;
        asteroidRotateAnglePerSecond *= inverseSpin ? -1f : 1f;
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        Rotate();
    }
    #endregion

    #region Custom Methods
    protected override void EnemyVisible() { }
    protected override void EnemyInvisible() { }

    protected override void EnemyCollide(Collider2D collision)
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


    // Move downwards using world axis
    protected override void Move()
    {
        transform.Translate(Vector3.down * asteroidMoveSpeed * Time.deltaTime, Space.World);
    }

    // Rotate GameObject using the Z axis
    private void Rotate()
    {
        transform.Rotate(Vector3.forward * asteroidRotateAnglePerSecond * Time.deltaTime);
    }


    // IDamageable - Behaviour to run when the Asteroid has no health left
    public override void Death()
    {
        AddScore();

        // Instantiate explosion and disable this GameObject after 0.1f seconds
        GameObject explosion = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Explosion);
        explosion.transform.position = transform.position;
        explosion.transform.rotation = Quaternion.identity;

        base.Death();

        DisableEnemy(.1f);
    }
    #endregion
}