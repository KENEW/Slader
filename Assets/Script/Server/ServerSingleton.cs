using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ServerSingleton<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
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
                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }
}