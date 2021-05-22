using UnityEngine;

public class Laser : LaserBase
{
    [SerializeField] private bool up;

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
            gameObject.SetActive(false);
        }

        if (collision.gameObject.TryGetComponent(out PowerUp powerup))
        {
            powerup.Explode();
            gameObject.SetActive(false);
        }
    }

    protected override void Move()
    {
        // Moves the laser
        transform.Translate((up ? Vector3.up : Vector3.down) * laserSpeed * Time.deltaTime);

        // If Laser is out of bounds
        if (Mathf.Abs(transform.position.y) >= SpaceShooterData.LaserBoundLimitsY || Mathf.Abs(transform.position.x) >= SpaceShooterData.WrapX)
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
}