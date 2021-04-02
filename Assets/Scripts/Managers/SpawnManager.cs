using System.Collections;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    #region Variables
    // Shared Spawner Properties
    private enum SpawnEntity { Enemy, PowerUp }
    private WaitForSeconds spawnRate = new WaitForSeconds(5f);


    [Header("Enemy Spawner")]
    [SerializeField] private GameObject enemyPrefab;
    private Coroutine enemyRespawnCoroutine;


    [Header("PowerUp Spawner")]
    [SerializeField] private GameObject powerUpPrefab;
    private Coroutine powerUpRespawnCoroutine;
    #endregion


    #region MonoBehaviour Methods
    // Start is called before the first frame update
    private void Start()
    {
        StartEntitySpawn(SpawnEntity.Enemy);
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
        StopEntitySpawn(SpawnEntity.PowerUp);
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(Random.Range(-SpaceShooterData.EnemySpawnX, SpaceShooterData.EnemySpawnX), SpaceShooterData.EnemyBoundLimitsY.y, 0f), Quaternion.identity);
            newEnemy.transform.SetParent(transform);
            yield return spawnRate;
        }
    }
    private IEnumerator SpawnPowerUp()
    {
        while (true)
        {
            PowerUp newPowerUp = Instantiate(powerUpPrefab, new Vector3(Random.Range(-SpaceShooterData.EnemySpawnX * .9f, SpaceShooterData.EnemySpawnX * .9f), SpaceShooterData.EnemyBoundLimitsY.y, 0f), Quaternion.identity).GetComponent<PowerUp>();
            int randomPowerUp = Random.Range(0, System.Enum.GetNames(typeof(PowerUp.Type)).Length);
            newPowerUp.SetPowerupType((PowerUp.Type)randomPowerUp);
            yield return spawnRate;
        }
    }
    #endregion
}