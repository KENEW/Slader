using System.Collections;
using Photon.Pun;
using UnityEngine;

public class ExpendabilityItem : MonoBehaviourPunCallbacks
{
	[SerializeField] protected float sustainTime = 15.0f;
	[SerializeField] protected string objectName;
	
	public override void OnEnable()
	{
		StartCoroutine(SustainDestroy());
	}
	[PunRPC]
	public void DestroyObject()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			ObjectManager.Instance.itemPool.ObjectEnqueue(objectName, this.gameObject);
		}
	}
	IEnumerator SustainDestroy()
	{
		yield return new WaitForSeconds(sustainTime);
		photonView.RPC(nameof(DestroyObject), RpcTarget.All);
	}
}
