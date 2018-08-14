using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : class
{
    public static T Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            throw new SingletonNotInitializedException();
        }
    }

    private static T _instance;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = GetComponent<T>();
        }
        else
        {
            throw new SingletonAlreadyInitializedException();
        }
    }
}
