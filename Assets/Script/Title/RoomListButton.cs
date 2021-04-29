using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// 로비에서 방 목록을 확인할 때 나타내는 버튼을 처리해준다.
/// </summary>
public class RoomListButton : MonoBehaviour
{
    public Text RoomNameText;
    public Text RoomPlayersText;
    public Button JoinRoomButton;

    private string roomName;

    public void Start()
    {
        JoinRoomButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SoundManager.Instance.PlaySFX("ButtonClick");

            LobbyManager.Instance.OnRoomReadyScreen();
            PhotonNetwork.JoinRoom(roomName);
        });
    }
    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        roomName = name;

        RoomNameText.text = name;
        RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
    }
}
