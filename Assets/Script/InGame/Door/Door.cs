using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Door : MonoBehaviourPunCallbacks
{
	public Image doorHpGauge;
	public Image backGauge;

	private int curHp;
	private int maxHp;

	private bool backCheck = false;

	private void Start()
	{
		curHp = maxHp = 150;
	}
	private void Update()
	{
		doorHpGauge.fillAmount = Mathf.Lerp(doorHpGauge.fillAmount, ((float)curHp / (float)maxHp), 7.5f * Time.deltaTime);

		if (backCheck)
		{
			backGauge.fillAmount = Mathf.Lerp(backGauge.fillAmount, doorHpGauge.fillAmount, 3.5f * Time.deltaTime);

			if (doorHpGauge.fillAmount >= backGauge.fillAmount - 0.01f)
			{
				backCheck = false;
				backGauge.fillAmount = doorHpGauge.fillAmount;
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Enemy"))
		{
			if(PhotonNetwork.IsMasterClient)
			{
				collision.GetComponent<PhotonView>().RPC("SetKillDirectly", RpcTarget.All, false);
				collision.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);

				photonView.RPC(nameof(GetDamage), RpcTarget.All, 10);
			}
		}
	}
	[PunRPC]
	public void GetDamage(int damage)
	{
		if (GameManager.Instance.gameState != GameState.GameOver)
		{
			if (curHp - damage <= 0)
			{
				SoundManager.Instance.PlaySFX("DoorBreak");
				curHp = 0;
				photonView.RPC(nameof(ManaStoneBreak), RpcTarget.All);
			}
			else
			{
				SoundManager.Instance.PlaySFX("DoorHit");
				curHp -= damage;
			}

			Invoke(nameof(BackGagueRun), 0.1f);
		}
	}
	[PunRPC]
	public void RecoveryHp(int value)
	{
		if (GameManager.Instance.gameState != GameState.GameOver)
		{
			if (curHp + value >= maxHp)
			{
				curHp = maxHp;
			}
			else
			{
				curHp += value;
			}
		}
	}
	[PunRPC]
	public void ManaStoneBreak()
	{
		if (GameManager.Instance.gameState == GameState.GameStart)
		{
			GameManager.Instance.GameOver();
		}
	}
	private void BackGagueRun()
	{
		backCheck = true;
	}
}
