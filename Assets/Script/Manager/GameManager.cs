using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum GameState
{
    GameWaiting,
    GameReady,
    GameStart,
    GameOver,
    GameClear
}
public class GameManager : ServerSingleton<GameManager>
{
    //GameReuslt coin acquire scale
    private readonly float CLEAR_COIN_RESULT = 1.3f;
    private readonly float FAIL_COIN_RESULT = 0.7f;

    public GameState gameState = GameState.GameWaiting;
    public int RemainStartCount;

    private void Awake()
    {
        int screenX = Screen.width - (Screen.width / 6);
        int screenY = Screen.height - (Screen.height / 6);

        Screen.SetResolution(screenX, screenY, true);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        GameInit();
        Hashtable props = new Hashtable
        {
            {SladerGame.PLAYER_LOADED_LEVEL, true}
        };
		PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public void GameInit()
    {
        CoinManager.Instance.CoinLoad();
        ScoreManager.Instance.ScoreInit();
    }
    [PunRPC]
    public void GameStart()
    {
        gameState = GameState.GameStart;

        SoundManager.Instance.PlayBGM("InGame");
        UIManager.Instance.OnStartScreen();
        ScoreManager.Instance.ScoreInit();
        WaveManager.Instance.WaveStart();
    }
    public void GameOver()
    {
        SoundManager.Instance.PlaySFX("GameOver");

        gameState = GameState.GameOver;

        MyData.Instance.IsAutoServerQuit = false;

        var players = FindObjectsOfType<Player>();
        foreach(Player player in players)
        {
            player.state = PlayerState.Death;
		}


            MonsterDestroy();
        
      
        int addCoin = (int)(ScoreManager.Instance.GetScore() * FAIL_COIN_RESULT);
        MyData.Instance.MyCoin = addCoin;
        MyData.Instance.charData.highWave = WaveManager.Instance.GetCurrentWave - 1;
        MyData.Instance.charData.gameCount++;
        
        ServerData.Instance.SaveData();
        DataController.Instance.SaveGameData();

        SoundManager.Instance.StopBGM();
        UIManager.Instance.OnGameOver(addCoin, ScoreManager.Instance.GetScore());
    }
    public void GameClear()
    {
        SoundManager.Instance.PlaySFX("GameClear");

        gameState = GameState.GameClear;

        MyData.Instance.IsAutoServerQuit = false;

        int addCoin = (int)(ScoreManager.Instance.GetScore() * CLEAR_COIN_RESULT);
        MyData.Instance.MyCoin = addCoin;
        MyData.Instance.charData.highWave = WaveManager.Instance.GetCurrentWave - 1;
        MyData.Instance.charData.gameCount++;

        ServerData.Instance.SaveData();
        DataController.Instance.SaveGameData();

        SoundManager.Instance.StopBGM();
        UIManager.Instance.OnGameClear(addCoin, ScoreManager.Instance.GetScore());
	}
    IEnumerator CountTimeStartCo(Action act)
    {
        RemainStartCount = 10;

        while (RemainStartCount >= 0)
        {
            yield return new WaitForSeconds(1.0f);
            RemainStartCount--;
		}

        act();
	}
    [PunRPC]
    private void MonsterDestroy()
    {
       var monsters = FindObjectsOfType<Monster>();

        foreach(var monster in monsters)
        {
           monster.gameObject.SetActive(false);
        }
    }
    private bool AllPlayerLoadedSceneCheck()
    {
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(SladerGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
	{
        //룸 안에 모든 플레이어가 씬 로드에 성공하면
        if (changedProps.ContainsKey(SladerGame.PLAYER_LOADED_LEVEL))
        {
            if (AllPlayerLoadedSceneCheck())
            {
                gameState = GameState.GameReady;
                UIManager.Instance.OnGameReady();
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(CountTimeStartCo(() => { photonView.RPC(nameof(GameStart), RpcTarget.All); }));
                }
            }
        }
    }
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
        if (stream.IsWriting)
        {
            stream.SendNext(RemainStartCount);
        }
        else
        {
            RemainStartCount = (int)stream.ReceiveNext();
        }
    }
}