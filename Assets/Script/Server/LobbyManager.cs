using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class LobbyManager : ServerSingleton<LobbyManager>
{
	private Dictionary<string, GameObject> panelList = new Dictionary<string, GameObject>();
	private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
	private Dictionary<string, GameObject> roomListEntries = new Dictionary<string, GameObject>();
	private Dictionary<int, GameObject> playerListEntries = new Dictionary<int, GameObject>();

	public GameObject RoomListContent;
	public GameObject PlayerListInsideContent;

	public GameObject RoomListEntryPrefab;
	public GameObject PlayerListEntryPrefab;    //플레이어 리스트 컨텐츠 프리팹

	public Button[] sceneDisableButton;
	public Button startGameButton;
	public InputField RoomNameInputField;

	[Header("Panel List")]
	public GameObject LobbyPanel;
	public GameObject TitlePanel;
	public GameObject RoomListPanel;
	public GameObject RoomCreatePanel;
	public GameObject RoomReadyPanel;
	public GameObject RoomLobbyPanel;

	public Text versionText;

	private readonly string gameVersion = "0.1";
	private bool isSingle = false;

	private void Start()
	{
		versionText.text = "version " + gameVersion;

		panelList.Add("RoomListPanel", RoomListPanel);
		panelList.Add("RoomCreatePanel", RoomCreatePanel);
		panelList.Add("RoomReadyPanel", RoomReadyPanel);
		panelList.Add("TitlePanel", TitlePanel);
		panelList.Add("LobbyPanel", LobbyPanel);

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();	
		}

		PhotonNetwork.OfflineMode = false;
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.GameVersion = gameVersion;

		PhotonNetwork.NickName = MyData.Instance.charData.name;
	}
	public void OnSinglePlayButtonClicked()
	{
		SoundManager.Instance.PlaySFX("ButtonClick");

		if (PhotonNetwork.IsConnected)
		{
			isSingle = true;
			PhotonNetwork.Disconnect();
		}
		
		StartCoroutine(DisConnectCallback(() =>
		{
			PhotonNetwork.OfflineMode = true;

			RoomOptions options = new RoomOptions {MaxPlayers = 1, PlayerTtl = 10000};
			PhotonNetwork.CreateRoom("SinglePlay", options, null);
			PhotonNetwork.CurrentRoom.IsOpen = false;
			PhotonNetwork.CurrentRoom.IsVisible = false;

			PhotonNetwork.LoadLevel("InGame");
			foreach(var button in sceneDisableButton)
			{
				button.interactable = false;
			}
		}));
	}
	public IEnumerator DisConnectCallback(Action act)
	{
		while (PhotonNetwork.IsConnected)
		{
			yield return null;
		}
		act();
	}
	public void OnRoomListButtonClicked()
	{
		SoundManager.Instance.PlaySFX("ButtonClick");

		if (!PhotonNetwork.InLobby)
		{ 
			PhotonNetwork.JoinLobby();
		}

		SetActivePanelOn("RoomListPanel");
	}
	public void OnCreateRoomButtonClicked()
	{
		string roomName = RoomNameInputField.text;
		roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;
		RoomOptions options = new RoomOptions { MaxPlayers = 2, PlayerTtl = 10000 };

		PhotonNetwork.CreateRoom(roomName, options, null);

		OnRoomReadyScreen();
	}
	public void SetActivePanelOn(string panelName)
	{
		panelList[panelName].SetActive(true);
	}
	public void SetActivePanelOff(string panelName)
	{
		panelList[panelName].SetActive(false);
	}
	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		ClearRoomListView();

		UpdateCachedRoomList(roomList);
		UpdateRoomListView();
	}
	public override void OnDisconnected(DisconnectCause cause)
	{
		if (!isSingle)
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}
	/// <summary>
	/// 방안에 접속했을 때 드로우 콜을 줄이기 위해 나머지 패널들을 비활성화시킨다.
	/// </summary>
	public void OnRoomReadyScreen()
	{
		SetActivePanelOff("RoomCreatePanel");
		SetActivePanelOff("LobbyPanel");
		SetActivePanelOff("TitlePanel");

		SetActivePanelOn("RoomReadyPanel");
	}
	public override void OnJoinedRoom()
	{
		cachedRoomList.Clear();
	
		if (playerListEntries == null)
		{
			playerListEntries = new Dictionary<int, GameObject>();
		}

		foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
		{
			GameObject entry = Instantiate(PlayerListEntryPrefab);
			entry.transform.SetParent(PlayerListInsideContent.transform);
			entry.transform.localScale = Vector3.one;
			entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

			object isPlayerReady;
			if (p.CustomProperties.TryGetValue(SladerGame.PLAYER_READY, out isPlayerReady))
			{
				entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
			}

			playerListEntries.Add(p.ActorNumber, entry);
		}

		startGameButton.interactable = CheckPlayersReady();

		Hashtable props = new Hashtable
			{
				{SladerGame.PLAYER_LOADED_LEVEL, false}
			};
		PhotonNetwork.LocalPlayer.SetCustomProperties(props);
	}
	private void ClearRoomListView()
	{
		foreach (GameObject entry in roomListEntries.Values)
		{
			Destroy(entry.gameObject);
		}

		roomListEntries.Clear();
	}
	private void UpdateCachedRoomList(List<RoomInfo> roomList)
	{
		foreach (RoomInfo info in roomList)
		{
			// Remove room from cached room list if it got closed, became invisible or was marked as removed
			if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
			{
				if (cachedRoomList.ContainsKey(info.Name))
				{
					cachedRoomList.Remove(info.Name);
				}

				continue;
			}

			// Update cached room info
			if (cachedRoomList.ContainsKey(info.Name))
			{
				cachedRoomList[info.Name] = info;
			}
			else
			{
				cachedRoomList.Add(info.Name, info);
			}
		}
	}
	private void UpdateRoomListView()
	{
		foreach (RoomInfo info in cachedRoomList.Values)
		{
			GameObject entry = Instantiate(RoomListEntryPrefab);
			entry.transform.SetParent(RoomListContent.transform);
			entry.transform.localScale = Vector3.one;
			entry.GetComponent<RoomListButton>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

			roomListEntries.Add(info.Name, entry);
		}
	}
	public override void OnJoinedLobby()
	{
		OnConnectCheckPopup();

		cachedRoomList.Clear();
		ClearRoomListView();
	}
	public override void OnLeftLobby()
	{
		OnConnectCheckPopup();

		cachedRoomList.Clear();
		ClearRoomListView();
	}
	private bool CheckPlayersReady()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return false;
		}

		foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
		{
			object isPlayerReady;

			if (p.CustomProperties.TryGetValue(SladerGame.PLAYER_READY, out isPlayerReady))
			{
				if (!(bool)isPlayerReady)
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		return true;
	}
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		OnConnectCheckPopup();

		GameObject entry = Instantiate(PlayerListEntryPrefab);
		entry.transform.SetParent(PlayerListInsideContent.transform);
		entry.transform.localScale = Vector3.one;
		entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

		playerListEntries.Add(newPlayer.ActorNumber, entry);

		startGameButton.interactable = CheckPlayersReady();
	}
	public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
	{
		OnConnectCheckPopup();
		
		if (playerListEntries == null)
		{
			playerListEntries = new Dictionary<int, GameObject>();
		}

		GameObject entry;
		if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
		{
			object isPlayerReady;
			if (changedProps.TryGetValue(SladerGame.PLAYER_READY, out isPlayerReady))
			{
				entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
			}
		}

		startGameButton.interactable = CheckPlayersReady();
	}
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		OnConnectCheckPopup();
	}
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		OnConnectCheckPopup();
	}
	public override void OnLeftRoom()
	{
		OnConnectCheckPopup();

		foreach (GameObject entry in playerListEntries.Values)
		{
			Destroy(entry.gameObject);
		}

		playerListEntries.Clear();
		playerListEntries = null;
	}
	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		OnConnectCheckPopup();

		Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
		playerListEntries.Remove(otherPlayer.ActorNumber);

		startGameButton.interactable = CheckPlayersReady();
	}
	public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
	{
		OnConnectCheckPopup();

		if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
		{
			startGameButton.interactable = CheckPlayersReady();
		}
	}
	public void OnBackButtonClicked()
	{
		OnConnectCheckPopup();
		
		if (PhotonNetwork.InLobby)
		{
			PhotonNetwork.LeaveLobby();
		}

		SetActivePanelOff("RoomListPanel");
	}
	public void OnLeaveGameButtonClicked()
	{
		SoundManager.Instance.PlaySFX("ButtonClick");

		OnConnectCheckPopup();
		PhotonNetwork.LeaveRoom();

		SetActivePanelOn("TitlePanel");
		SetActivePanelOn("LobbyPanel");
		SetActivePanelOff("RoomReadyPanel");
	}
	public void OnStartGameButtonClicked()
	{
		OnConnectCheckPopup();
		SoundManager.Instance.PlaySFX("ButtonClick");

		foreach (var button in sceneDisableButton)
		{
			button.interactable = false;
		}

		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;

		PhotonNetwork.LoadLevel("InGame");
	}
	/// <summary>
	/// 기기에서 포톤 연결이 끊겨있을 때 팝업창을 띄운다.
	/// </summary>
	private void OnConnectCheckPopup()
	{
		if (!PhotonNetwork.IsConnected)
		{
			WarningPopup.Instance.OpenPopup("RoomDisconnect");
		}
	}
}