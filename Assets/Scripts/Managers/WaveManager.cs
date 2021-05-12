using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Wave
{
    [Range(0f, 15f)] public float nextWaveSpawnTime;
    public uint minEnemiesLeftToProgress;

    [Space(10)]

    public List<WaveEnemySpawnProperties> enemiesToSpawn;
}

[System.Serializable]
public class WaveEnemySpawnProperties
{
    public enum WaveSpawnMode { Random, CustomPosition, CustomRotation, CustomTransform }

    [SerializeField] private string name;
    public SpaceShooterData.Enemies enemyToSpawn;

    [Space(10)]

    [Range(0f, 10f)] public float spawnTime;

    [Space(10)]

    public WaveSpawnMode spawnMode;
    public Vector2 customSpawnPos = new Vector2(0f, SpaceShooterData.EnemyBoundLimitsY);
    [Range(-180f, 180f)] public float customZRot;

}

public class WaveManager : MonoSingleton<WaveManager>
{
    [SerializeField] private float initializeNewDelay;
    [SerializeField] private List<WaveDataSO> waveList;
    private List<IDamageable> enemiesOnScene = new List<IDamageable>();
    private Coroutine wavesCoroutine;
    private Coroutine currentWaveCoroutine;


    public System.Action<int> OnWaveStart;
    public System.Action OnAllWavesCompleted;


    // Start is called before the first frame update
    void Start()
    {
        wavesCoroutine = StartCoroutine(WavesCoroutine());
    }

    public void StopWaveCoroutine()
    {
        if (wavesCoroutine != null)
        {
            StopCoroutine(wavesCoroutine);
            wavesCoroutine = null;
        }
        if (currentWaveCoroutine != null)
        {
            StopCoroutine(currentWaveCoroutine);
            currentWaveCoroutine = null;
        }
    }

    private IEnumerator WavesCoroutine()
    {
        yield return new WaitForSeconds(initializeNewDelay);

        for (int i = 0; i < waveList.Count; i++)
        {
            OnWaveStart?.Invoke(i);

            currentWaveCoroutine = StartCoroutine(CurrentWaveCoroutine(i));
            yield return new WaitUntil(() => currentWaveCoroutine == null); 

            yield return new WaitUntil(() => enemiesOnScene.Count <= waveList[i].wave.minEnemiesLeftToProgress);
            yield return new WaitForSeconds(waveList[i].wave.nextWaveSpawnTime);
        }

        OnAllWavesCompleted?.Invoke();
    }
    private IEnumerator CurrentWaveCoroutine(int currentWave)
    {
        yield return new WaitForSeconds(initializeNewDelay);
        List<WaveEnemySpawnProperties> enemiesToSpawn = waveList[currentWave].wave.enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            GameObject enemySpawned = null;

            switch (enemiesToSpawn[i].spawnMode)
            {
                case WaveEnemySpawnProperties.WaveSpawnMode.Random:
                    enemySpawned = SpawnManager.Instance.SpawnEnemy(enemiesToSpawn[i].enemyToSpawn);
                    break;

                case WaveEnemySpawnProperties.WaveSpawnMode.CustomPosition:
                    enemySpawned = SpawnManager.Instance.SpawnEnemy(enemiesToSpawn[i].enemyToSpawn, enemiesToSpawn[i].customSpawnPos);
                    break;

                case WaveEnemySpawnProperties.WaveSpawnMode.CustomRotation:
                    enemySpawned = SpawnManager.Instance.SpawnEnemy(enemiesToSpawn[i].enemyToSpawn, Quaternion.Euler(new Vector3(0f, 0f, enemiesToSpawn[i].customZRot)));
                    break;

                case WaveEnemySpawnProperties.WaveSpawnMode.CustomTransform:
                    enemySpawned = SpawnManager.Instance.SpawnEnemy(enemiesToSpawn[i].enemyToSpawn, enemiesToSpawn[i].customSpawnPos, Quaternion.Euler(new Vector3(0f, 0f, enemiesToSpawn[i].customZRot)));
                    break;
            }

            if (enemySpawned.TryGetComponent(out IDamageable entity))
            {
                enemiesOnScene.Add(entity);
                entity.OnEntityKilled += EnemyKilled;
            }

            yield return new WaitForSeconds(enemiesToSpawn[i].spawnTime);
        }

        currentWaveCoroutine = null;
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
