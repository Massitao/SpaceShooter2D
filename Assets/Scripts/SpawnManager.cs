using System.Collections;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [Header("Shared Spawner Properties")]
    [SerializeField] private WaitForSeconds spawnRate = new WaitForSeconds(5f);
    [SerializeField] private Vector2 respawnBoundX;
    [SerializeField] private float respawnHeightY;


    [Header("Enemy Spawner")]
    [SerializeField] private GameObject enemyPrefab;
    private Coroutine enemyRespawnCoroutine;


    [Header("PowerUp Spawner")]
    [SerializeField] private GameObject powerUpPrefab;
    private Coroutine powerUpRespawnCoroutine;


    // Start is called before the first frame update
    private void Start()
    {
        StartEnemySpawn();
        StartPowerUpSpawn();
    }

    public void StartEnemySpawn()
    {
        if (enemyRespawnCoroutine == null)
        {
            enemyRespawnCoroutine = StartCoroutine(SpawnEnemy());
        }
    }
    public void StopEnemySpawn()
    {
        if (enemyRespawnCoroutine != null)
        {
            StopCoroutine(enemyRespawnCoroutine);
            enemyRespawnCoroutine = null;
        }
    }
    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(Random.Range(respawnBoundX.x, respawnBoundX.y), respawnHeightY, 0f), Quaternion.identity);
            newEnemy.transform.SetParent(transform);
            yield return spawnRate;
        }
    }


    public void StartPowerUpSpawn()
    {
        if (powerUpRespawnCoroutine == null)
        {
            powerUpRespawnCoroutine = StartCoroutine(SpawnPowerUp());
        }
    }
    public void StopPowerUpSpawn()
    {
        if (powerUpRespawnCoroutine != null)
        {
            StopCoroutine(powerUpRespawnCoroutine);
            powerUpRespawnCoroutine = null;
        }
    }
    private IEnumerator SpawnPowerUp()
    {
        while (true)
        {
            PowerUp newPowerUp = Instantiate(powerUpPrefab, new Vector3(Random.Range(respawnBoundX.x, respawnBoundX.y), respawnHeightY, 0f), Quaternion.identity).GetComponent<PowerUp>();
            int randomPowerUp = Random.Range(0, System.Enum.GetNames(typeof(PowerUp.Type)).Length);
            newPowerUp.SetPowerupType((PowerUp.Type)randomPowerUp);
            yield return spawnRate;
        }
    }


    public void StopAllSpawns()
    {
        StopEnemySpawn();
        StopPowerUpSpawn();
    }
}