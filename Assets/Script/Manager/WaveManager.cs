using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Photon.Pun;

using static SladerGame;

public class WaveManager : ServerSingleton<WaveManager>, IPunObservable
{
	public SpawnPoint[] spawnPoint;

	public Text waveText;
	public Image waveProcessBar;

	//What the current wave monster is about
	private int remainMonsterNum = 0;
	private int waveMonsterNum = 0;
	private int waitingSpawnNum = 0;
	private int curMonsterNum = 0;

	private int curWave = 0;
	public int GetCurrentWave { get { return curWave; } set { curWave = value; } }

	private List<Dictionary<string, object>> waveMonsterInfo;	//wave monster number info
	private List<string> monsterList;   //Monster weight random list
	private Dictionary<string, string> monsterInfo;	//Monster resorce prefab name list

	private void Start()
	{		
		monsterInfo = new Dictionary<string, string>();
		monsterList = new List<string>();

		monsterInfo.Add("Blue_Slime", "BlueSlimeMonster");
		monsterInfo.Add("Green_Slime", "GreenSlimeMonster");
		monsterInfo.Add("Red_Slime", "RedSlimeMonster");
		monsterInfo.Add("Skeleton", "SkeletonMonster");
		monsterInfo.Add("Bat", "BatMonster");
		monsterInfo.Add("Devil", "DevilMonster");

		waveMonsterInfo = MyData.Instance.waveMonster.ToList();
		RemoveEmptyMonsterList();
	}

	/// <summary>
	/// 각 몬스터 스폰 수에서 0이 있는지 감지하고 있으면 리스트에서 삭제한다.
	/// </summary>
	public void RemoveEmptyMonsterList()
	{
		for (int wave = 0; wave < waveMonsterInfo.Count; wave++)
		{
			waveMonsterInfo[wave].Remove("Stage");

			for (int i = waveMonsterInfo[wave].Count - 1; i > 0; i--)
			{ 
				if((int)waveMonsterInfo[wave].ElementAt(i).Value <= 0)
				{ 
					string name = waveMonsterInfo[wave].ElementAt(i).Key;
					waveMonsterInfo[wave].Remove(name);
				}
			}
		}
	}

	[PunRPC]
	private void UIUpdate()
	{
		if (curWave >= 1)
		{
			waveText.text = "WAVE " + curWave;
			waveProcessBar.fillAmount = (((float)waveMonsterNum - (float)curMonsterNum) / (float)waveMonsterNum);
		}
	}

	[PunRPC]
	private void MonstersDestroy()
	{
		var monsters = FindObjectsOfType<Monster>();

		foreach (var monster in monsters)
		{
			monster.gameObject.SetActive(false);
		}
	}

	public void WaveStart()
	{
		SoundManager.Instance.PlaySFX("WaveStart");

		curWave++;
		remainMonsterNum = 0;

		photonView.RPC(nameof(UIUpdate), RpcTarget.All);

		monsterList.Clear();
		foreach (KeyValuePair<string, object> pair in waveMonsterInfo[curWave - 1])
		{
			remainMonsterNum += (int)pair.Value;
			for (int i = 0; i < (int)pair.Value; i++)
			{
				monsterList.Add(pair.Key);
			}
		}

		waitingSpawnNum = waveMonsterNum = curMonsterNum = remainMonsterNum;

		if (PhotonNetwork.IsMasterClient)
		{
			foreach (SpawnPoint point in spawnPoint)
			{
				point.MonsterSpawn();
			}
		}

		UIManager.Instance.OnWaveStart(curWave);
		GameManager.Instance.gameState = GameState.GameStart;
	}

	/// <summary>
	/// 현재 남은 몬스터 수에서 1만큼 감소한다.
	/// </summary>
	public void MonsterDeath()
	{
		if (GameManager.Instance.gameState == GameState.GameStart)
		{
			curMonsterNum--;
			if (curMonsterNum <= 0)
			{
				photonView.RPC(nameof(WaveClear), RpcTarget.All);
			}

			photonView.RPC(nameof(UIUpdate), RpcTarget.All);
		}
	}

	public string MonsterSpawn()
	{
		if (waitingSpawnNum <= 0)
		{
			DebugOptimum.Log("예외처리 - BlueMonster소환");
			return "BlueSlimeMonster";
		}

		waitingSpawnNum--;

		int randomIndex = Random.Range(0, monsterList.Count);
		string mosnterName = monsterList[randomIndex];
		monsterList.RemoveAt(randomIndex);

		return monsterInfo[mosnterName];
	}

	/// <summary>
	/// 남은 몬스터가 0 이하인지 확인한다.
	/// </summary>
	public bool CheckMonsterNum()
	{
		return waitingSpawnNum > 0 ? true : false;
	}

	[PunRPC]
	private void WaveClear()
	{
		GameManager.Instance.gameState = GameState.GameClear;

		if (curWave < waveMonsterInfo.Count)
		{
			Invoke(nameof(WaveStart), 6.5f);

			var players = FindObjectsOfType<Player>();
			foreach (Player player in players)
			{
				player.state = PlayerState.Live;
			}

			UIManager.Instance.OnWaveClear();
			SoundManager.Instance.PlaySFX("WaveClear");
		}
		else
		{
			GameManager.Instance.GameClear();
			PlayerManager.Instance.GetPlayer().playerHp.RecoveryHp(MAX_HP);
		}
	}
	
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(curMonsterNum);
			stream.SendNext(waveMonsterNum);
		}
		else
		{
			curMonsterNum = (int)stream.ReceiveNext();
			waveMonsterNum = (int)stream.ReceiveNext();
		}
	}
}