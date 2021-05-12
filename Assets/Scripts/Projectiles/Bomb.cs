using UnityEngine;

public class Bomb : LaserBase
{
    [Header("Bomb Properties")]
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float distanceToExplode;

    [SerializeField] private uint bombLasersToInstantiate;

    private Vector2 spawnPos;
    private float distanceTravelled;


    private void OnEnable()
    {
        spawnPos = transform.position;
    }

    private void OnDisable()
    {
        distanceTravelled = 0f;
    }

    private void Update()
    {
        Move();
    }

    private void BombExplode()
    {
        GameObject laser = null;
        float anglePerLaser = 360f / bombLasersToInstantiate;
        float totalAngle = -anglePerLaser;

        for (int i = 0; i < bombLasersToInstantiate; i++)
        {
            laser = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.EnemyLaser, transform.position, Quaternion.Euler(0f, 0f, totalAngle));
            totalAngle += anglePerLaser;
        }

        gameObject.SetActive(false);
    }

    protected override void Move()
    {
        transform.Translate(-transform.up * Mathf.Lerp(laserSpeed, laserSpeed * 0.25f, speedCurve.Evaluate(distanceTravelled / distanceToExplode)) * Time.deltaTime);
        distanceTravelled = Vector2.Distance(spawnPos, transform.position);

        if (distanceTravelled >= distanceToExplode)
        {
            BombExplode();
        }

        // If Laser is out of bounds
        if (Mathf.Abs(transform.position.y) >= SpaceShooterData.LaserBoundLimitsY || Mathf.Abs(transform.position.x) >= SpaceShooterData.WrapX)
        {
            gameObject.SetActive(false);
        }
    }


    protected override void LaserCollision(Collider2D collision)
    {
        // Hurts any Damageable Entity
        if (collision.gameObject.TryGetComponent(out IDamageable damageableEntity))
        {
            damageableEntity.TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}