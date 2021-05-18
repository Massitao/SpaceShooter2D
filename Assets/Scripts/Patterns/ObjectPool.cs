using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string name;

    [Space(5)]

    public ObjectPool.PoolType poolType;
    public GameObject prefabToInstantiate;
    [HideInInspector] public GameObject poolContainer;

    [Space(5)]

    public Queue<GameObject> pooledGameObjects = new Queue<GameObject>();
    public int populateAmount;
}

public class ObjectPool : MonoSingleton<ObjectPool>
{
    [SerializeField] private List<Pool> poolList = new List<Pool>();

    public enum PoolType { Rookie, Bomber, Toxic, Asteroid, Explosion, ToxicSmoke, PlayerLaser, TripleShotLaser, HeatSeekLaser, EnemyLaser, Bomb, Powerup }
    private Dictionary<PoolType, Pool> poolDictionary = new Dictionary<PoolType, Pool>();


    // Start is called before the first frame update
    private void Start()
    {
        PopulatePools();
    }
    private void PopulatePools()
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (!poolDictionary.ContainsKey(poolList[i].poolType))
            {
                poolDictionary.Add(poolList[i].poolType, poolList[i]);
                poolList[i].poolContainer = new GameObject($"{poolList[i].poolType.ToString()} Pool");      // Sets a container for the pooled objects
                poolList[i].poolContainer.transform.parent = transform;

                for (int j = 0; j < poolList[i].populateAmount; j++)
                {
                    GenerateNewObject(poolList[i].poolType);
                }
            }
            else
            {
                Debug.LogError($"Pool {i} has a repeated Pool Type: {poolList[i].poolType.ToString()}");
                continue;
            }
        }
    }

    private void GenerateNewObject(PoolType poolType)
    {
        if (poolDictionary.ContainsKey(poolType))
        {
            // Instantiate Object inside the respective Pool Container
            GameObject newObject = Instantiate(poolDictionary[poolType].prefabToInstantiate, poolDictionary[poolType].poolContainer.transform);

            // Add tracker to the Object
            newObject.AddComponent<ObjectPoolTracker>().SetObjectType(poolType);

            // Queues the object to its respective pool (Tracker calls ReturnPooledObject())
            newObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Requested Object from a non existing pool! {poolType.ToString()}");
        }
    }

    public GameObject GetPooledObject(PoolType poolType, Vector3 pos, Quaternion rot)
    {
        if (poolDictionary.ContainsKey(poolType))
        {
            // Dequeue object and activates it
            if (poolDictionary[poolType].pooledGameObjects.Count != 0)
            {
                GameObject pooledObject = poolDictionary[poolType].pooledGameObjects.Dequeue();
                pooledObject.transform.position = pos;
                pooledObject.transform.rotation = rot;

                pooledObject.SetActive(true);
                return pooledObject;
            }

            // Creates a new object and instantly returns it
            else
            {
                GenerateNewObject(poolType);

                GameObject pooledObject = poolDictionary[poolType].pooledGameObjects.Dequeue();
                pooledObject.transform.position = pos;
                pooledObject.transform.rotation = rot;

                pooledObject.SetActive(true);
                return pooledObject;
            }
        }
        else
        {
            Debug.LogWarning($"Requested Object from a non existing pool! {poolType.ToString()}");
            return null;
        }
    }
    public void ReturnPooledObject(ObjectPoolTracker objectToReturn)
    {
        if (poolDictionary.ContainsKey(objectToReturn.GetObjectType()))
        {
            // Queues object
            poolDictionary[objectToReturn.GetObjectType()].pooledGameObjects.Enqueue(objectToReturn.gameObject);
        }
        else
        {
            Debug.LogWarning($"Returned Object belongs to a non existing pool! {objectToReturn.GetObjectType().ToString()}");
            Destroy(objectToReturn.gameObject);
        }
    }
}