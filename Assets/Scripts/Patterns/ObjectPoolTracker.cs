using UnityEngine;

public class ObjectPoolTracker : MonoBehaviour
{
    private ObjectPool.PoolType objectType;


    private void OnDisable()
    {
        ReturnToObjectPool(false);
    }

    public ObjectPool.PoolType GetObjectType()
    {
        return objectType;
    }
    public void SetObjectType(ObjectPool.PoolType type)
    {
        objectType = type;
    }

    public void ReturnToObjectPool(bool force)
    {
        if (ObjectPool.Instance != null)
        {
            if (force)  gameObject.SetActive(false);
            else        ObjectPool.Instance.ReturnPooledObject(this);
        }
        else
        {
            Debug.LogWarning($"Trying to return an Object to a Pool, but the ObjectPool Manager doesn't exist!");
        }
    }
}