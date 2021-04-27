using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
	Effect,
	Projectile,
	Object
}

[System.Serializable]
public class ObjectInfo
{
	public string poolName;
	public int poolCount;

	public GameObject prefab;
	public Transform parentTrans;

	[HideInInspector]
	public Queue<GameObject> objQueue = new Queue<GameObject>();
}
public class PoolManager : MonoBehaviour
{
	public ObjectInfo[] objectInfo;

	private void Start()
	{
		QueueInit();
	}
	private void QueueInit()
	{
		for(int i = 0; i < objectInfo.GetLength(0); i++)
		{
			InsertQueue(objectInfo[i]);
		}
	}
	private void InsertQueue(ObjectInfo objectInfo)
	{
		for(int i = 0; i < objectInfo.poolCount; i++)
		{
			GameObject pool = Instantiate(objectInfo.prefab, transform.position, Quaternion.identity);
			pool.SetActive(false);
			objectInfo.objQueue.Enqueue(pool);

			if (objectInfo.parentTrans != null)
			{
				pool.transform.SetParent(objectInfo.parentTrans);
			}
			else
			{
				pool.transform.SetParent(transform);
			}
		}
	}
	public GameObject ObjectDequeue(string t_poolName)
	{
		for (int i = 0; i < objectInfo.GetLength(0); i++)
		{
			if (t_poolName == objectInfo[i].poolName)
			{
				var t_obj = objectInfo[i].objQueue.Dequeue();
				t_obj.SetActive(true);
				return t_obj;
			}
		}

		DebugOptimum.Log("오브젝트를 찾을 수 없습니다.");
		return null;
	}
	public bool ObjectEnqueue(string poolName, GameObject obj)
	{
		for(int i = 0; i < objectInfo.GetLength(0); i++)
		{
			if(poolName == objectInfo[i].poolName)
			{
				objectInfo[i].objQueue.Enqueue(obj);
				obj.SetActive(false);
				return true;
			}
		}
		DebugOptimum.Log("오브젝트를 찾을 수 없습니다.");
		return false;
	}
	public void ObjectEnqueue(string poolName, GameObject obj, Action act)
	{
		for(int i = 0; i < objectInfo.GetLength(0); i++)
		{
			if(poolName == objectInfo[i].poolName)
			{
				objectInfo[i].objQueue.Enqueue(obj);
				obj.SetActive(false);
				act();
			}
		}
		DebugOptimum.Log("오브젝트를 찾을 수 없습니다.");
	}
	public void ObjectEnqueue(string poolName, GameObject obj, float delayTime)
	{
		StartCoroutine(ObjectEnqueueCo(poolName, obj, delayTime));
	}
	public int GetObjectCount()
	{
		return objectInfo.Length;
	}
	private IEnumerator ObjectEnqueueCo(string poolName, GameObject obj, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		ObjectEnqueue(poolName, obj);
	}
}
