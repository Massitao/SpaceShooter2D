using System.Collections;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    #region Variables
    // Defined Entities to Spawn
    private enum SpawnEntity { Rookie, Asteroid, PowerUp }


    [Header("Rookie Spawner")]
    private Coroutine enemyRespawnCoroutine;
    private WaitForSeconds enemySpawnRate = new WaitForSeconds(2f);


    [Header("Asteroid Spawner")]
    private Coroutine asteroidRespawnCoroutine;
    private WaitForSeconds asteroidSpawnRate = new WaitForSeconds(5f);


    [Header("PowerUp Spawner")]
    private Coroutine powerUpRespawnCoroutine;
    private WaitForSeconds powerUpSpawnRate = new WaitForSeconds(7f);
    #endregion


    #region MonoBehaviour Methods
    // Start is called before the first frame update
    private void Start()
    {
        StartEntitySpawn(SpawnEntity.Rookie);
        StartEntitySpawn(SpawnEntity.Asteroid);
        StartEntitySpawn(SpawnEntity.PowerUp);
    }
    #endregion

    #region Custom Methods
    private void StartEntitySpawn(SpawnEntity spawnEntity)
    {
        switch (spawnEntity)
        {
            case SpawnEntity.Rookie:
                if (enemyRespawnCoroutine == null)
                {
                    enemyRespawnCoroutine = StartCoroutine(SpawnEnemyEntity(spawnEntity, enemySpawnRate));
                }

                break;

            case SpawnEntity.Asteroid:
                if (asteroidRespawnCoroutine == null)
                {
                    asteroidRespawnCoroutine = StartCoroutine(SpawnEnemyEntity(spawnEntity, asteroidSpawnRate));
                }

                break;

            case SpawnEntity.PowerUp:
                if (powerUpRespawnCoroutine == null)
                {
                    powerUpRespawnCoroutine = StartCoroutine(SpawnEnemyEntity(spawnEntity, powerUpSpawnRate));
                }

                break;
        }
    }
    private void StopEntitySpawn(SpawnEntity spawnEntity)
    {
        switch (spawnEntity)
        {
            case SpawnEntity.Rookie:
                if (enemyRespawnCoroutine != null)
                {
                    StopCoroutine(enemyRespawnCoroutine);
                    enemyRespawnCoroutine = null;
                }

                break;

            case SpawnEntity.Asteroid:
                if (asteroidRespawnCoroutine != null)
                {
                    StopCoroutine(asteroidRespawnCoroutine);
                    asteroidRespawnCoroutine = null;
                }

                break;

            case SpawnEntity.PowerUp:
                if (powerUpRespawnCoroutine != null)
                {
                    StopCoroutine(powerUpRespawnCoroutine);
                    powerUpRespawnCoroutine = null;
                }

                break;
        }
    }
    public void StopAllSpawns()
    {
        StopEntitySpawn(SpawnEntity.Rookie);
        StopEntitySpawn(SpawnEntity.Asteroid);
        StopEntitySpawn(SpawnEntity.PowerUp);
    }

    private IEnumerator SpawnEnemyEntity(SpawnEntity spawnEntity, WaitForSeconds spawnDelay)
    {
        Vector2 spawnPos = Vector2.zero;

        while (true)
        {
            yield return spawnDelay;

            switch (spawnEntity)
            {
                case SpawnEntity.Rookie:

                    spawnPos = new Vector3(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY.y);

                    GameObject newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Rookie);
                    newEnemy.transform.position = spawnPos;
                    newEnemy.transform.rotation = Quaternion.identity;

                    break;

                case SpawnEntity.Asteroid:

                    spawnPos = new Vector3(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY.y);

                    GameObject newAsteroid = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Asteroid);
                    newAsteroid.transform.position = spawnPos;
                    newAsteroid.transform.rotation = Quaternion.identity;

                    break;

                case SpawnEntity.PowerUp:

                    spawnPos = new Vector3(Random.Range(-SpaceShooterData.SpawnX * .9f, SpaceShooterData.SpawnX * .9f), SpaceShooterData.EnemyBoundLimitsY.y);

                    PowerUp newPowerUp = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Powerup).GetComponent<PowerUp>();
                    newPowerUp.transform.position = spawnPos;
                    newPowerUp.transform.rotation = Quaternion.identity;

                    int randomPowerUp = Random.Range(0, System.Enum.GetNames(typeof(PowerUp.Type)).Length);
                    randomPowerUp = (randomPowerUp == (int)PowerUp.Type.HeatSeek && Random.value > 0.5f) ? (int)PowerUp.Type.TripleShot : randomPowerUp;

                    newPowerUp.SetPowerupType((PowerUp.Type)randomPowerUp);

                    break;
            }
        }
    }
    #endregion
}