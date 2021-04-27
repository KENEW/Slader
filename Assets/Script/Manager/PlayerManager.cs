using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerManager : ServerSingleton<PlayerManager>
{
    public GameObject[] userHpBar = new GameObject[2];
    public Text[] userNameText = new Text[2];
    public Player[] player = new Player[2];

	public GameObject playerObj;

    public int actorNum;
    public int playerNum;
    
    [SerializeField] private Vector2[] spawnPos = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0)
    };

    #region UNITY
    private void Start()
    {
        actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        playerNum = actorNum - 1;
        
        if (playerObj == null)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber == -1)
            {
                PlayerSpawn(0);
            }
            else
            {
                PlayerSpawn(PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }
    public void PlayerSpawn(int playerNum)
    {
        if (playerObj != null)
        {
            return;
        }
        if(playerNum == 2)
        {
            NetworkManager.Instance.IsCoop = true;
        }
        playerNum--;

        playerObj = PhotonNetwork.Instantiate("Prefab/Player/" + "Player", spawnPos[playerNum], Quaternion.identity);
        CameraController.Instance.SetTarget(playerNum, playerObj.transform);
        
        playerObj.GetComponent<PhotonView>().RPC("Init", RpcTarget.All, playerNum, spawnPos[playerNum]);
        playerObj.GetComponent<PhotonView>().RPC("SetUpgradeInfo", RpcTarget.All, 
            MyData.Instance.charData.manaLevel, 
            MyData.Instance.charData.attackLevel,
            MyData.Instance.charData.coinLevel);
        
        photonView.RPC("HpSetActive", RpcTarget.All, playerNum, PhotonNetwork.NickName);
    }
    public Player GetPlayer()
    {
        var players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.playerNum == this.playerNum)
            {
                return player;
            }
        }
        return null;
    }
    public Player GetPlayer(int playerNum)
    {
        var players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.playerNum == playerNum)
            {
                return player;
            }
        }
        return null;
    }

    #endregion

    #region PUN_CALLBACK

    [PunRPC]
    public void HpSetActive(int playerNum, string playerName)
    {
        userHpBar[playerNum].SetActive(true);
 
        userHpBar[playerNum].GetComponent<PlayerHp>().Init(playerNum);
        userHpBar[playerNum].GetComponent<PhotonView>().TransferOwnership(playerNum + 1);
        userNameText[playerNum].text = playerName;
    }
    [PunRPC]
    public void SetPlayerState(MoveState playerState)
    {
        var players = FindObjectsOfType<Player>();
        foreach(Player player in players)
        {
            player.moveState = playerState;
		}
	}
    #endregion
}