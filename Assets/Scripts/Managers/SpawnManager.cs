using System.Collections;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    #region Variables
    // Defined Entities to Spawn
    private enum SpawnEntity { Enemy, Asteroid, PowerUp }


    [Header("Enemy Spawner")]
    [SerializeField] private GameObject enemyPrefab;
    private Coroutine enemyRespawnCoroutine;
    private WaitForSeconds enemySpawnRate = new WaitForSeconds(2f);


    [Header("Asteroid Spawner")]
    [SerializeField] private GameObject asteroidPrefab;
    private Coroutine asteroidRespawnCoroutine;
    private WaitForSeconds asteroidSpawnRate = new WaitForSeconds(5f);


    [Header("PowerUp Spawner")]
    [SerializeField] private GameObject powerUpPrefab;
    private Coroutine powerUpRespawnCoroutine;
    private WaitForSeconds powerUpSpawnRate = new WaitForSeconds(7f);
    #endregion


    #region MonoBehaviour Methods
    // Start is called before the first frame update
    private void Start()
    {
        StartEntitySpawn(SpawnEntity.Enemy);
        StartEntitySpawn(SpawnEntity.Asteroid);
        StartEntitySpawn(SpawnEntity.PowerUp);
    }
    #endregion

    #region Custom Methods
    private void StartEntitySpawn(SpawnEntity spawnEntity)
    {
        switch (spawnEntity)
        {
            case SpawnEntity.Enemy:
                if (enemyRespawnCoroutine == null)
                {
                    enemyRespawnCoroutine = StartCoroutine(SpawnEnemyEntity(enemyPrefab, enemySpawnRate));
                }

                break;

            case SpawnEntity.Asteroid:
                if (asteroidRespawnCoroutine == null)
                {
                    asteroidRespawnCoroutine = StartCoroutine(SpawnEnemyEntity(asteroidPrefab, asteroidSpawnRate));
                }

                break;

            case SpawnEntity.PowerUp:
                if (powerUpRespawnCoroutine == null)
                {
                    powerUpRespawnCoroutine = StartCoroutine(SpawnPowerUp());
                }

                break;
        }
    }
    private void StopEntitySpawn(SpawnEntity spawnEntity)
    {
        switch (spawnEntity)
        {
            case SpawnEntity.Enemy:
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
        StopEntitySpawn(SpawnEntity.Enemy);
        StopEntitySpawn(SpawnEntity.Asteroid);
        StopEntitySpawn(SpawnEntity.PowerUp);
    }

    private IEnumerator SpawnEnemyEntity(GameObject enemyToSpawn, WaitForSeconds spawnDelay)
    {
        Vector2 spawnPos = Vector2.zero;

        while (true)
        {
            yield return spawnDelay;

            spawnPos = new Vector3(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY.y);
            GameObject newEnemy = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity, transform);
        }
    }
    private IEnumerator SpawnPowerUp()
    {
        Vector2 spawnPos = Vector2.zero;

        while (true)
        {
            yield return powerUpSpawnRate;

            spawnPos = new Vector3(Random.Range(-SpaceShooterData.SpawnX * .9f, SpaceShooterData.SpawnX * .9f), SpaceShooterData.EnemyBoundLimitsY.y);
            PowerUp newPowerUp = Instantiate(powerUpPrefab, spawnPos, Quaternion.identity, transform).GetComponent<PowerUp>();

            int randomPowerUp = Random.Range(0, System.Enum.GetNames(typeof(PowerUp.Type)).Length);
            randomPowerUp = (randomPowerUp == (int)PowerUp.Type.HeatSeek && Random.value > 0.5f) ? (int)PowerUp.Type.TripleShot : randomPowerUp;

            newPowerUp.SetPowerupType((PowerUp.Type)randomPowerUp);
        }
    }
    #endregion
}