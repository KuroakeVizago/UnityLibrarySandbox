using UnityEngine;

public class Singleton<T> where T : class
{
    public T Instance { get; private set; }

    public Singleton(T instance)
    {
        Instance = instance;
    }

    ~Singleton()
    {
        Debug.Log(Instance + " Destroyed");
    }
    
}