using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 싱글톤 - 어디서든 접근 가능함 (다른 씬에서도 작동)
/// </summary>
public class SceneSingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = FindObjectOfType(typeof(T)) as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static T Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }

            return instance;
        }
    }
}