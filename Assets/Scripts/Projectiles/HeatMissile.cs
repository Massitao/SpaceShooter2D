using System.Collections;
using UnityEngine;

public class HeatMissile : LaserBase
{
    [Header("Heat Missile Movement")]
    [SerializeField] private float angularSpeed = 5f;

    [SerializeField] private bool worldAxis;
    [SerializeField] private Vector2 moveDir;

    [Header("Target")]
    [SerializeField] private float radiusDetection = 8f;
    [SerializeField] private LayerMask detectionLM;

    private EnemyBase enemyTarget = null;
    private Vector2 enemyTargetDir = Vector2.zero;

    private Collider2D[] enemyContacts = new Collider2D[10];
    private WaitForSeconds heatSeekUpdate = new WaitForSeconds(0.5f);


    #region MonoBehaviour Methods
    private void OnEnable()
    {
        StartCoroutine(HeatSeek());
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize Target Detection for Debug purposes
        Gizmos.DrawWireSphere(transform.position, radiusDetection);
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
    }


    protected override void Move()
    {
        // If the Missile has a target, get a direction and rotate the GameObject towards the target
        if (enemyTarget != null)
        {
            enemyTargetDir = (enemyTarget.transform.position - transform.position).normalized;
            transform.Rotate(0f, 0f, Vector3.Cross(enemyTargetDir, transform.up).z * -1 * angularSpeed * Time.deltaTime);
        }

        // Move upwards (local axis)
        transform.Translate(moveDir.normalized * laserSpeed * Time.deltaTime, worldAxis ? Space.World : Space.Self);

        // Wrap ship around X axis
        if (Mathf.Abs(transform.position.x) > SpaceShooterData.WrapX)
        {
            transform.position = new Vector3(SpaceShooterData.WrapX * Mathf.Sign(transform.position.x) * -1, transform.position.y);
        }

        // If Laser is out of bounds
        if (Mathf.Abs(transform.position.y) >= SpaceShooterData.LaserBoundLimitsY)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator HeatSeek()
    {
        // Variables needed to compare between selectable targets
        EnemyBase   selectedEnemy   = null;                 // If the target selection has found a new entity, it will be stored here
        float       nearestDistance = radiusDetection;      // Selects the nearest target to this missile
        float       distanceCheck   = 0f;                   // Every target distance will be calculated here
        int         enemiesFound    = 0;                    // Amount of enemies found by the OverlapCircle

        // This coroutine will always be active until the missile has either completed or failed its objective.
        while (true)
        {
            // If there's currently no target selected or the target is dead, start retargeting
            if (enemyTarget == null || enemyTarget.EntityHealth == 0)
            {
                // Clear Temporary Selected Enemy
                selectedEnemy = null;

                // Reset Nearest Distance
                nearestDistance = radiusDetection;

                // Gets the number of enemies found, and stores the results inside enemyContacts
                enemiesFound = Physics2D.OverlapCircleNonAlloc(transform.position, radiusDetection, enemyContacts, detectionLM);

                // For every Contact made...
                for (int i = 0; i < enemyContacts.Length; i++)
                {
                    // Check if the Contact is valid...
                    if (enemyContacts[i] != null)
                    {
                        // Check if it's an enemy and if it still has health...
                        if (enemyContacts[i].TryGetComponent(out EnemyBase enemyToCheck) && enemyToCheck.EntityHealth > 0)
                        {
                            if (enemyToCheck is Asteroid) continue;

                            // Get distance between the missile and the target
                            distanceCheck = Vector3.Distance(transform.position, enemyContacts[i].transform.position);

                            // Check if the distance is lower than the previous target nearest distance...
                            if (distanceCheck < nearestDistance)
                            {
                                // If true, selects new target and updates the nearest distance
                                selectedEnemy = enemyToCheck;
                                nearestDistance = distanceCheck;
                            }
                        }
                    }
                }

                // Set the target to the nearest enemy found
                enemyTarget = selectedEnemy;
            }

            yield return heatSeekUpdate;
        }
    }
    #endregion
}