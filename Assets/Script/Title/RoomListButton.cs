using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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
