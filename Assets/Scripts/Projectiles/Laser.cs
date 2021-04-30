using UnityEngine;

public class Laser : LaserBase
{
    #region Variables
    [Header("Laser Movement")]
    [SerializeField] private bool worldAxis;
    [SerializeField] private Vector2 moveDir;
    #endregion


    #region MonoBehaviour Methods
    // Update is called once per frame
    private void Update()
    {
        Move();
    }
    #endregion

    #region Custom Methods
    protected override void LaserCollision(Collider2D collision)
    {
        // Hurts any Damageable Entity
        if (collision.gameObject.TryGetComponent(out IDamageable damageableEntity))
        {
            damageableEntity.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    protected override void Move()
    {
        // Moves the laser
        transform.Translate(moveDir.normalized * laserSpeed * Time.deltaTime, worldAxis ? Space.World : Space.Self);

        // If Laser is out of bounds
        if (transform.position.y <= SpaceShooterData.LaserBoundLimitsY.x || transform.position.y >= SpaceShooterData.LaserBoundLimitsY.y)
        {
            Destroy(gameObject);
        }
    }
    #endregion
}