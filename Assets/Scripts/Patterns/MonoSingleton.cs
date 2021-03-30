using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log($"No instance found! Creating new one");
                instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }     
    }

    protected virtual void Init() { }
}
