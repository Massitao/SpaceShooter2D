using System.Collections;
using UnityEngine;

public class HeatMissile : MonoBehaviour
{
    [Header("Heat Missile Properties")]
    [SerializeField] private float missileSpeed = 10f;
    [SerializeField] private float angularSpeed = 5f;

    [Header("Target")]
    [SerializeField] private float radiusDetection = 8f;
    [SerializeField] private LayerMask detectionLM;

    private Enemy enemyTarget = null;
    private Vector2 enemyTargetDir = Vector2.zero;

    private Collider2D[] enemyContacts = new Collider2D[5];
    private WaitForSeconds heatSeekUpdate = new WaitForSeconds(0.5f);
    private Coroutine heatSeekCoroutine = null;


    #region MonoBehaviour Methods
    private void Start()
    {
        heatSeekCoroutine = StartCoroutine(HeatSeek());
    }

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
            damageableEntity.TakeDamage(1);
            StopHeatSeek();
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radiusDetection);
    }
    #endregion

    #region Custom Methods
    private void Move()
    {
        if (enemyTarget != null)
        {
            enemyTargetDir = (enemyTarget.transform.position - transform.position).normalized;
            transform.Rotate(0f, 0f, Vector3.Cross(enemyTargetDir, transform.up).z * -1 * angularSpeed * Time.deltaTime);
        }

        // Move upwards (local axis)
        transform.Translate(Vector3.up * missileSpeed * Time.deltaTime, Space.Self);
        
        // If Laser is out of bounds
        if (transform.position.y <= SpaceShooterData.LaserBoundLimitsY.x || transform.position.y >= SpaceShooterData.LaserBoundLimitsY.y)
        {
            StopHeatSeek();
            Destroy(gameObject);
        }
    }

    IEnumerator HeatSeek()
    {
        Enemy selectedEnemy = null;
        float nearestDistance = radiusDetection;
        float distanceCheck = 0f;

        while (true)
        {
            if (enemyTarget == null)
            {
                selectedEnemy = null;
                nearestDistance = radiusDetection;
                int enemiesFound = Physics2D.OverlapCircleNonAlloc(transform.position, radiusDetection, enemyContacts, detectionLM);

                for (int i = 0; i < enemyContacts.Length; i++)
                {
                    if (enemyContacts[i] != null)
                    {
                        if (enemyContacts[i].TryGetComponent(out Enemy enemyToCheck) && enemyToCheck.EntityHealth != 0)
                        {
                            distanceCheck = Vector3.Distance(transform.position, enemyContacts[i].transform.position);
                            if (distanceCheck < nearestDistance)
                            {
                                selectedEnemy = enemyToCheck;
                                nearestDistance = distanceCheck;
                            }
                        }
                    }
                }
            }

            enemyTarget = selectedEnemy;
            yield return heatSeekUpdate;
        }
    }
    private void StopHeatSeek()
    {
        if (heatSeekCoroutine != null)
        {
            StopCoroutine(heatSeekCoroutine);
            heatSeekCoroutine = null;
        }
    }
    #endregion
}