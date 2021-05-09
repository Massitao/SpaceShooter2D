using System.Collections;
using System.Collections.Generic;
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

        switch (enemyToSpawn)
        {
            case SpaceShooterData.Enemies.Rookie:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Rookie);
                break;
        }

        newEnemy.transform.position = new Vector2(Random.Range(-SpaceShooterData.SpawnX, SpaceShooterData.SpawnX), SpaceShooterData.EnemyBoundLimitsY.y);
        newEnemy.transform.rotation = Quaternion.identity;

        return newEnemy;
    }
    public GameObject SpawnEnemy(SpaceShooterData.Enemies enemyToSpawn, Vector2 pos)
    {
        GameObject newEnemy = null;

        switch (enemyToSpawn)
        {
            case SpaceShooterData.Enemies.Rookie:
                newEnemy = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Rookie);
                break;
        }

        newEnemy.transform.position = pos;
        newEnemy.transform.rotation = Quaternion.identity;

        return newEnemy;
    }
    private IEnumerator SpawnPowerUp()
    {
        while (true)
        {
            yield return powerUpSpawnRate;

            PowerUp newPowerUp = ObjectPool.Instance.GetPooledObject(ObjectPool.PoolType.Powerup).GetComponent<PowerUp>();
            newPowerUp.transform.position = new Vector3(Random.Range(-SpaceShooterData.SpawnX * .9f, SpaceShooterData.SpawnX * .9f), SpaceShooterData.EnemyBoundLimitsY.y);
            newPowerUp.transform.rotation = Quaternion.identity;

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