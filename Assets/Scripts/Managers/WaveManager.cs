using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Wave
{
    public List<WaveEnemySpawnProperties> enemiesToSpawn;

    public float nextWaveSpawnTime;
    public int minEnemiesLeftToProgress;
}

[System.Serializable]
public class WaveEnemySpawnProperties
{
    [SerializeField] private string name;
    public SpaceShooterData.Enemies enemyToSpawn;

    public float spawnTime;

    public bool useCustomSpawnPoint;
    public Vector2 customSpawnPoint;
}

public class WaveManager : MonoSingleton<WaveManager>
{
    [SerializeField] private List<Wave> waves;
    [SerializeField] private List<IDamageable> enemiesOnScene = new List<IDamageable>();
    [SerializeField] private float initWaveDelay;

    public System.Action<int> OnWaveStart;
    public System.Action OnAllWavesCompleted;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WavesCoroutine());
    }

    private IEnumerator WavesCoroutine()
    {
        yield return new WaitForSeconds(initWaveDelay);

        for (int i = 0; i < waves.Count; i++)
        {
            OnWaveStart?.Invoke(i);
            yield return StartCoroutine(CurrentWaveCoroutine(i));

            yield return new WaitUntil(() => enemiesOnScene.Count <= waves[i].minEnemiesLeftToProgress);
            yield return new WaitForSeconds(waves[i].nextWaveSpawnTime);
        }

        OnAllWavesCompleted?.Invoke();
    }
    private IEnumerator CurrentWaveCoroutine(int currentWave)
    {
        yield return new WaitForSeconds(initWaveDelay);
        List<WaveEnemySpawnProperties> enemiesToSpawn = waves[currentWave].enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            GameObject enemySpawned = null;

            if (enemiesToSpawn[i].useCustomSpawnPoint)
            {
                enemySpawned = SpawnManager.Instance.SpawnEnemy(enemiesToSpawn[i].enemyToSpawn, enemiesToSpawn[i].customSpawnPoint);
            }
            else
            {
                enemySpawned = SpawnManager.Instance.SpawnEnemy(enemiesToSpawn[i].enemyToSpawn);
            }

            if (enemySpawned.TryGetComponent(out IDamageable entity))
            {
                enemiesOnScene.Add(entity);
                entity.OnEntityKilled += EnemyKilled;
            }

            yield return new WaitForSeconds(enemiesToSpawn[i].spawnTime);
        }
    }

    private void EnemyKilled(IDamageable enemyKilled)
    {
        if (enemiesOnScene.Contains(enemyKilled))
        {
            enemiesOnScene.Remove(enemyKilled);
            enemyKilled.OnEntityKilled -= EnemyKilled;
        }
    }
}
