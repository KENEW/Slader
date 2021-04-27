using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
