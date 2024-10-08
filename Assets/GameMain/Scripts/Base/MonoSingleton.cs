using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance && Instance != GetComponent<T>())
        {
            Destroy(gameObject);
            return;
        }

        Instance = GetComponent<T>();
    }
}