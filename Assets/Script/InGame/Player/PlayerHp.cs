using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHp : MonoBehaviourPunCallbacks
{
	private Player player;

	public Image hpGauge;
	public Image hpGaugeBack;

	private int curHp;
	private int maxHp;

	private float respawnTime = 10.0f;
	private bool backHpDamage = false;

	[PunRPC]
	public void Init(int playerNum)
	{
		player = PlayerManager.Instance.GetPlayer(playerNum);
		maxHp = curHp = player.GetManaSoul();
		
		DebugOptimum.Log("현재 체력 : " + curHp + ", 최대 체력 : " + maxHp);
	}
	private void Update()
	{
		hpGaugeBack.fillAmount = Mathf.Lerp(hpGaugeBack.fillAmount, hpGauge.fillAmount, 3.5f * Time.deltaTime);

		if (hpGauge.fillAmount >= hpGaugeBack.fillAmount - 0.01f)
		{
			backHpDamage = false;
			hpGaugeBack.fillAmount = hpGauge.fillAmount;
		}

		hpGauge.fillAmount = Mathf.Lerp(hpGauge.fillAmount, ((float)curHp / (float)maxHp), 7.5f * Time.deltaTime);
	}
	#region PUN CALLBACKS
	[PunRPC]
	public void GetDamage(int damage)
	{
		if (GameManager.Instance.gameState == GameState.GameStart)
		{
			if (curHp - damage <= 0)
			{
				curHp = 0;
				BackHpRun();
				photonView.RPC(nameof(PlayerDeath), RpcTarget.All);
			}
			else
			{
				curHp -= damage;
				Invoke(nameof(BackHpRun), 0.05f);

				if (photonView.IsMine)
				{
					if (HpRatioCheck(0.35f))
					{
						FadingHealth.Instance.FadingTrigger(0.2f);
					}
					else
					{
						FadingHealth.Instance.FadingStart(0.4f);
					}
				}
			}
		}
	}
	[PunRPC]
	public void RecoveryHp(int value)
	{
		if (curHp + value > maxHp)
		{
			curHp = maxHp;
		}
		else
		{
			curHp += value;
		}

		if (photonView.IsMine)
		{
			if (HpRatioCheck(0.35f))
			{
				FadingHealth.Instance.FadingStop();
			}
		}
	}
	#endregion

	private void BackHpRun()
	{
		backHpDamage = true;
	}
	private bool HpRatioCheck(float dest)
	{
		if ((float)curHp / (float)maxHp >= dest)
		{
			return true;
		}

		return false;
	}
	[PunRPC]
	public void PlayerDeath()
	{
		if(player.state == PlayerState.Live)
		{
			player.state = PlayerState.Death;
			StartCoroutine(DeathCo());

			player.NoFightImg.SetActive(true);
			if (player.transform.localScale.x == -1)
			{
				player.NoFightImg.transform.localScale = new Vector3(-1, 1, 1);
			}
			else
			{
				player.NoFightImg.transform.localScale = new Vector3(1, 1, 1);

			}

			if(photonView.IsMine)
			{
				FadingHealth.Instance.FadingStop();
				UIManager.Instance.DeathScreenView(10);
			}
		}
	}
	[PunRPC]
	public void ReSpawn()
	{
		player.state = PlayerState.Live;
		
		player.NoFightImg.SetActive(false);
		RecoveryHp(maxHp);
		player.OnSpawn();
		FadingHealth.Instance.FadingStop();
	}
	private IEnumerator DeathCo()
	{
		float curTime = 0.0f;
		
		while (curTime <= respawnTime)
		{
			if (GameManager.Instance.gameState == GameState.GameClear)
			{
				break;
			}
			curTime += 0.1f;
			yield return new WaitForSeconds(0.1f);
		}

		if (GameManager.Instance.gameState != GameState.GameOver)
		{
			photonView.RPC(nameof(ReSpawn), RpcTarget.All);
		}
	}
}