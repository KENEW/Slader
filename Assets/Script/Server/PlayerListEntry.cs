using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;

public class PlayerListEntry : MonoBehaviour
{
    public Text PlayerNameText;

    public Image PlayerColorImage;
    public Image PlayerReadyImage;
    public Button PlayerReadyButton;

    public Sprite[] readyImg;

    private int ownerId;
    private bool isPlayerReady;

    public void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            PlayerReadyButton.interactable = false;
        }
        else
        {
			Hashtable initialProps = new Hashtable() { { SladerGame.PLAYER_READY, isPlayerReady }};

            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);

            PlayerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);

                Hashtable props = new Hashtable() { { SladerGame.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            });
        }
    }
    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
    }
    public void SetPlayerReady(bool playerReady)
    {
        PlayerReadyImage.sprite = playerReady ? readyImg[1] : readyImg[0];
    }
}
