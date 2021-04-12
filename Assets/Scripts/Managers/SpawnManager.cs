using System.Collections;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    #region Variables
    // Shared Spawner Properties
    private enum SpawnEntity { Enemy, Asteroid, PowerUp }
    private WaitForSeconds enemySpawnRate = new WaitForSeconds(2f);
    private WaitForSeconds asteroidSpawnRate = new WaitForSeconds(5f);
    private WaitForSeconds powerUpSpawnRate = new WaitForSeconds(10f);


    [Header("Enemy Spawner")]
    [SerializeField] private GameObject enemyPrefab;
    private Coroutine enemyRespawnCoroutine;


    [Header("Enemy Spawner")]
    [SerializeField] private GameObject asteroidPrefab;
    private Coroutine asteroidRespawnCoroutine;


    [Header("PowerUp Spawner")]
    [SerializeField] private GameObject powerUpPrefab;
    private Coroutine powerUpRespawnCoroutine;
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
                    enemyRespawnCoroutine = StartCoroutine(SpawnEnemy());
                }

                break;

            case SpawnEntity.Asteroid:
                if (asteroidRespawnCoroutine == null)
                {
                    asteroidRespawnCoroutine = StartCoroutine(SpawnAsteroid());
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

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return enemySpawnRate;

            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(Random.Range(-SpaceShooterData.EnemySpawnX, SpaceShooterData.EnemySpawnX), SpaceShooterData.EnemyBoundLimitsY.y, 0f), Quaternion.identity);
            newEnemy.transform.SetParent(transform);
        }
    }
    private IEnumerator SpawnAsteroid()
    {
        while (true)
        {
            yield return asteroidSpawnRate;

            GameObject newEnemy = Instantiate(asteroidPrefab, new Vector3(Random.Range(-SpaceShooterData.EnemySpawnX, SpaceShooterData.EnemySpawnX), SpaceShooterData.EnemyBoundLimitsY.y, 0f), Quaternion.identity);
            newEnemy.transform.SetParent(transform);
        }
    }
    private IEnumerator SpawnPowerUp()
    {
        while (true)
        {
            yield return powerUpSpawnRate;

            PowerUp newPowerUp = Instantiate(powerUpPrefab, new Vector3(Random.Range(-SpaceShooterData.EnemySpawnX * .9f, SpaceShooterData.EnemySpawnX * .9f), SpaceShooterData.EnemyBoundLimitsY.y, 0f), Quaternion.identity).GetComponent<PowerUp>();
            int randomPowerUp = Random.Range(0, System.Enum.GetNames(typeof(PowerUp.Type)).Length);

            if (randomPowerUp == (int)PowerUp.Type.HeatSeek)
            {
                if (Random.value > 0.5f)
                {
                    randomPowerUp = (int)PowerUp.Type.TripleShot;
                }
            }

            newPowerUp.SetPowerupType((PowerUp.Type)randomPowerUp);
        }
    }
    #endregion
}