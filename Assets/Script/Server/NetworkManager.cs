using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;

public class NetworkManager : ServerSingleton<NetworkManager>
{ 
	public bool IsCoop {set; get;}

	private void Start()
	{
		MyData.Instance.IsAutoServerQuit = true;
	}
	private void OnApplicationQuit()
	{
		if(PhotonNetwork.IsConnected)
		{
			DebugOptimum.Log("서버의 연결을 끊습니다.");
			PhotonNetwork.Disconnect();
		}
	}
	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		if(MyData.Instance.IsAutoServerQuit)
		{
			MyData.Instance.IsDisConnect = true;
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene("Lobby");
		}
	}
}
