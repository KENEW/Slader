using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 포톤을 이용한 오브젝트를 풀링을 사용할 때 RPC처리를 도와준다.
/// </summary>
public class PoolActive : MonoBehaviourPunCallbacks
{
	[PunRPC]
	public void SetActiveRPC(bool isActive)
	{
		gameObject.SetActive(isActive);
	}
	[PunRPC]
	public void SetPositionRPC(Vector2 pos)
	{
		transform.position = pos;
	}
}
