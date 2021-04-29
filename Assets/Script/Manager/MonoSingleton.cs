using UnityEngine;

/// <summary>
/// 싱글톤 - 어디서든 접근 가능함 (같은 씬에서만 작동)
/// </summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    instance = new GameObject("@" + typeof(T).ToString(),
                          typeof(T)).GetComponent<T>();
                }
            }
            return instance;
        }
    }
}