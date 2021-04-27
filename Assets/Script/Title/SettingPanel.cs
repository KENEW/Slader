using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SettingPanel : MonoBehaviour
{
	public GameObject settingPanel;

	public InputField inputName;
	public Text curName;

	public void OpenSettingPanel()
	{
		SoundManager.Instance.PlaySFX("ButtonClick");

		settingPanel.SetActive(true);
		curName.text = MyData.Instance.charData.name;
	}
	public void CloseSettingPanel()
	{
		SoundManager.Instance.PlaySFX("ButtonClick");

		settingPanel.SetActive(false);
		if(!inputName.text.Equals(""))
		{
			MyData.Instance.charData.name = inputName.text;
			PhotonNetwork.NickName = inputName.text;
		}

		ServerData.Instance.SaveData();
	}
}
