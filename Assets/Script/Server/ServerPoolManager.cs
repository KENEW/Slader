using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ServerPoolManager : MonoBehaviour
{
	[SerializeField] private string poolName;

	public ObjectInfo[] objectInfo;

	private void Start()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			QueueInit();
		}
	}
	private void QueueInit()
	{
		for (int i = 0; i < objectInfo.GetLength(0); i++)
		{
			InsertQueue(objectInfo[i]);
		}
	}
	private void InsertQueue(ObjectInfo t_objectInfo)
	{
		for (int i = 0; i < t_objectInfo.poolCount; i++)
		{
			GameObject t_pool = PhotonNetwork.Instantiate("Prefab/" + poolName + "/" + t_objectInfo.prefab.name, transform.position, Quaternion.identity);
			
			t_pool.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.All, false);
			t_objectInfo.objQueue.Enqueue(t_pool);

			if (t_objectInfo.parentTrans != null)
			{
				t_pool.transform.SetParent(t_objectInfo.parentTrans);
			}
			else
			{
				t_pool.transform.SetParent(this.transform);
			}
		}
	}
	public GameObject ObjectDequeue(string poolName, Vector2 position)
	{
		for (int i = 0; i < objectInfo.GetLength(0); i++)
		{
			if (poolName == objectInfo[i].poolName)
			{
				var t_obj = objectInfo[i].objQueue.Dequeue();
				t_obj.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.All, true);
				t_obj.GetComponent<PhotonView>().RPC("SetPositionRPC", RpcTarget.All, position);

				return t_obj;
			}
		}

		DebugOptimum.Log("오브젝트를 찾을 수 없습니다.");
		return null;
	}
	public void ObjectEnqueue(string t_poolName, GameObject t_object)
	{
		for (int i = 0; i < objectInfo.GetLength(0); i++)
		{
			if (t_poolName == objectInfo[i].poolName)
			{
				objectInfo[i].objQueue.Enqueue(t_object);
				//t_object.SetActive(false);
				t_object.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.All, false);
				return;
			}
		}

		DebugOptimum.Log("오브젝트를 찾을 수 없습니다.");
	}
	public void ObjectEnqueue(string t_poolName, GameObject t_object, float t_time)
	{
		StartCoroutine(ObjectEnqueueCo(t_poolName, t_object, t_time));
	}
	private IEnumerator ObjectEnqueueCo(string t_poolName, GameObject t_object, float t_time)
	{
		yield return new WaitForSeconds(t_time);
		ObjectEnqueue(t_poolName, t_object);
	}
}