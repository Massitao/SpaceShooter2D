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
    [SerializeField] private GameObject tripleShotPowerUpPrefab;
    private Coroutine tripleShotRespawnCoroutine;


    // Start is called before the first frame update
    private void Start()
    {
        StartEnemySpawn();
        StartTripleShotSpawn();
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


    public void StartTripleShotSpawn()
    {
        if (tripleShotRespawnCoroutine == null)
        {
            tripleShotRespawnCoroutine = StartCoroutine(SpawnTripleShot());
        }
    }
    public void StopTripleShotSpawn()
    {
        if (tripleShotRespawnCoroutine != null)
        {
            StopCoroutine(tripleShotRespawnCoroutine);
            tripleShotRespawnCoroutine = null;
        }
    }
    private IEnumerator SpawnTripleShot()
    {
        while (true)
        {
            Instantiate(tripleShotPowerUpPrefab, new Vector3(Random.Range(respawnBoundX.x, respawnBoundX.y), respawnHeightY, 0f), Quaternion.identity);
            yield return spawnRate;
        }
    }


    public void StopAllSpawns()
    {
        StopEnemySpawn();
        StopTripleShotSpawn();
    }
}