using System.Collections;
using UnityEngine;

[System.Serializable]
public class PowerupSpawn
{
    public string name;
    public PowerUp.Type powerUp;
    public int spawnWeight;
}

public class SpawnManager : MonoSingleton<SpawnManager>
{
    #region Variables
    [Header("PowerUp Spawner")]
    [SerializeField] private PowerupSpawn[] powerupTable;
    private int totalWeight;
    private int newWeight;

    private Coroutine powerUpRespawnCoroutine;
    private WaitForSeconds powerUpSpawnRate = new WaitForSeconds(7f);
    #endregion


    #region MonoBehaviour Methods
    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < powerupTable.Length; i++)
        {
            totalWeight += powerupTable[i].spawnWeight;
        }

        powerUpRespawnCoroutine = StartCoroutine(SpawnPowerUp());
    }
    #endregion

    #region Custom Methods
    public GameObject SpawnEnemy(SpaceShooterData.Enemies enemyToSpawn)
    {
        GameObject newEnemy = null;
        Vector3 randomSpawn = new Vector2(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY);
        Quaternion randomRot = (Random.value < 0.75f) ? Quaternion.identity : Quaternion.Euler(0f, 0f, Random.Range(0f, (transform.position.x <= 0f) ? SpaceShooterData.MaxEnemyRotation : -SpaceShooterData.MaxEnemyRotation));

        switch (enemyToSpawn)
        {
            case SpaceShooterData.Enemies.Rookie:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Rookie, randomSpawn, randomRot);
                break;

            case SpaceShooterData.Enemies.Bomber:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Bomber, randomSpawn, randomRot);
                break;

            case SpaceShooterData.Enemies.Toxic:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Toxic, randomSpawn, randomRot);
                break;

            case SpaceShooterData.Enemies.Reckless:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Reckless, randomSpawn, randomRot);
                break;

            case SpaceShooterData.Enemies.Watcher:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Watcher, randomSpawn, randomRot);
                break;

            case SpaceShooterData.Enemies.Avoider:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Avoider, randomSpawn, randomRot);
                break;

            case SpaceShooterData.Enemies.Collector:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Collector, randomSpawn, randomRot);
                break;

            case SpaceShooterData.Enemies.Asteroid:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Asteroid, randomSpawn, randomRot);
                break;
        }

        return newEnemy;
    }
    public GameObject SpawnEnemy(SpaceShooterData.Enemies enemyToSpawn, Vector2 pos)
    {
        GameObject newEnemy = null;
        Quaternion randomRot = (Random.value < 0.75f) ? Quaternion.identity : Quaternion.Euler(0f, 0f, Random.Range(0f, (transform.position.x <= 0f) ? SpaceShooterData.MaxEnemyRotation : -SpaceShooterData.MaxEnemyRotation));

        switch (enemyToSpawn)
        {
            case SpaceShooterData.Enemies.Rookie:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Rookie, pos, randomRot);
                break;

            case SpaceShooterData.Enemies.Bomber:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Bomber, pos, randomRot);
                break;

            case SpaceShooterData.Enemies.Toxic:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Toxic, pos, randomRot);
                break;

            case SpaceShooterData.Enemies.Reckless:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Reckless, pos, randomRot);
                break;

            case SpaceShooterData.Enemies.Watcher:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Watcher, pos, randomRot);
                break;

            case SpaceShooterData.Enemies.Avoider:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Avoider, pos, randomRot);
                break;

            case SpaceShooterData.Enemies.Collector:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Collector, pos, randomRot);
                break;

            case SpaceShooterData.Enemies.Asteroid:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Asteroid, pos, randomRot);
                break;
        }

        return newEnemy;
    }
    public GameObject SpawnEnemy(SpaceShooterData.Enemies enemyToSpawn, Quaternion rot)
    {
        GameObject newEnemy = null;
        Vector3 randomSpawn = new Vector2(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY);

        switch (enemyToSpawn)
        {
            case SpaceShooterData.Enemies.Rookie:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Rookie, randomSpawn, rot);
                break;

            case SpaceShooterData.Enemies.Bomber:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Bomber, randomSpawn, rot);
                break;

            case SpaceShooterData.Enemies.Toxic:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Toxic, randomSpawn, rot);
                break;

            case SpaceShooterData.Enemies.Reckless:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Reckless, randomSpawn, rot);
                break;

            case SpaceShooterData.Enemies.Watcher:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Watcher, randomSpawn, rot);
                break;

            case SpaceShooterData.Enemies.Avoider:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Avoider, randomSpawn, rot);
                break;

            case SpaceShooterData.Enemies.Collector:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Collector, randomSpawn, rot);
                break;

            case SpaceShooterData.Enemies.Asteroid:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Asteroid, randomSpawn, rot);
                break;
        }

        return newEnemy;
    }
    public GameObject SpawnEnemy(SpaceShooterData.Enemies enemyToSpawn, Vector2 pos, Quaternion rot)
    {
        GameObject newEnemy = null;

        switch (enemyToSpawn)
        {
            case SpaceShooterData.Enemies.Rookie:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Rookie, pos, rot);
                break;

            case SpaceShooterData.Enemies.Bomber:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Bomber, pos, rot);
                break;

            case SpaceShooterData.Enemies.Toxic:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Toxic, pos, rot);
                break;

            case SpaceShooterData.Enemies.Reckless:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Reckless, pos, rot);
                break;

            case SpaceShooterData.Enemies.Watcher:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Watcher, pos, rot);
                break;

            case SpaceShooterData.Enemies.Avoider:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Avoider, pos, rot);
                break;

            case SpaceShooterData.Enemies.Collector:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Collector, pos, rot);
                break;

            case SpaceShooterData.Enemies.Asteroid:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Asteroid, pos, rot);
                break;
        }

        return newEnemy;
    }

    private IEnumerator SpawnPowerUp()
    {
        Vector2 randomSpawn = Vector2.zero;

        while (true)
        {
            yield return powerUpSpawnRate;

            randomSpawn = new Vector2(Random.Range(-SpaceShooterData.SpawnX * .9f, SpaceShooterData.SpawnX * .9f), SpaceShooterData.EnemyBoundLimitsY);
            PowerUp newPowerUp = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Powerup, randomSpawn, Quaternion.identity).GetComponent<PowerUp>();
            newWeight = Random.Range(0, totalWeight);

            for (int i = 0; i < powerupTable.Length; i++)
            {
                if (newWeight <= powerupTable[i].spawnWeight)
                {
                    newPowerUp.SetPowerupType(powerupTable[i].powerUp);
                    break;
                }
                else
                {
                    newWeight -= powerupTable[i].spawnWeight;
                }
            }        
        }
    }

    public void StopAllSpawns()
    {
        StopCoroutine(powerUpRespawnCoroutine);
    }
    #endregion
}