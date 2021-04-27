using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LobbyCanvasManager : MonoBehaviour
{
	[Header("Canvas")]
	public GameObject titleCanvas;
	public GameObject companyCanvas;
	public GameObject lobbyCanvas;

	[Header("CompanyLogoCanvas")]
	public GameObject companyLogo;
	public Image companyLogoFadeImg;

	[Header("TitleCanvas")]
	public GameObject pressTouchText;
	public Animator logoAnimator;

	private void Awake()
	{
		//Screen.SetResolution(720, 1280, true);
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
	}
	private void Start()
	{
		if(!MyData.Instance.IsAuthInit)
		{
			companyCanvas.SetActive(true);

			CoroutineManager.Instance.CallWaitForSeconds(1.0f, () =>
			{
				companyLogo.SetActive(true);
				SoundManager.Instance.PlaySFX("GameStartButton");
				CoroutineManager.Instance.CallWaitForSeconds(2.0f, () =>
				{
					companyLogoFadeImg.DOFade(1f, 2.0f).OnComplete(() =>
					{
						titleCanvas.SetActive(true);
						companyCanvas.SetActive(false);
						SoundManager.Instance.PlayBGM("Lobby");
					});
				});
			});

			AuthManager.Instance.Init();
		}
		else
		{
			titleCanvas.SetActive(true);
		}
	}
	public void OnLobbyCanvas()
	{
		SoundManager.Instance.PlaySFX("ButtonClick");
		logoAnimator.SetBool("LogoUp", true);
		pressTouchText.SetActive(false);
		CoroutineManager.Instance.CallWaitForSeconds(1.0f, () => lobbyCanvas.SetActive(true));
	}
}
