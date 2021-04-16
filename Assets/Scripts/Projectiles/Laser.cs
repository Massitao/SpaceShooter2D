using UnityEngine;

public class Laser : MonoBehaviour
{
    #region Variables
    [Header("Laser Properties")]
    [SerializeField] private float laserSpeed = 6f;
    [SerializeField] private int damage = 1;
    #endregion


    #region MonoBehaviour Methods
    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Hurts any Damageable Entity
        if (collision.gameObject.TryGetComponent(out IDamageable damageableEntity))
        {
            damageableEntity.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
    #endregion

    #region Custom Methods
    private void Move()
    {
        // Move upwards (local axis)
        transform.Translate(Vector3.up * laserSpeed * Time.deltaTime, Space.Self);

        // If Laser is out of bounds
        if (transform.position.y <= SpaceShooterData.LaserBoundLimitsY.x || transform.position.y >= SpaceShooterData.LaserBoundLimitsY.y)
        {
            // If there's a parent, destroy it
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }
    #endregion
}