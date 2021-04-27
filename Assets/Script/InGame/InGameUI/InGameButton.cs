using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class InGameButton : MonoBehaviourPunCallbacks
{
    public GameObject weaponPanel;
    public WeaponSelect weaponSelect;

    public void OnWeaponPanel()
    {
        SoundManager.Instance.PlaySFX("ButtonClick");
        weaponPanel.SetActive(true);
    }
    public void OnTitleScene()
    {
        SoundManager.Instance.PlaySFX("ButtonClick");

        SceneManager.LoadScene("Lobby");
    }
    public void OnWeaponSelect()
    {
        SoundManager.Instance.PlaySFX("WeaponChange");
        photonView.RPC(nameof(WeaponSelectRPC), RpcTarget.All, PlayerManager.Instance.playerNum, weaponSelect.curWeaponID);
        weaponPanel.SetActive(false);
    }
    [PunRPC]
    public void WeaponSelectRPC(int playerNum, int weaponIndex)
    {
        PlayerManager.Instance.GetPlayer(playerNum).SetWeapon(weaponIndex);
    }
}