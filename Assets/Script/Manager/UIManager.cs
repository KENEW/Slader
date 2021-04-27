using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;

public class UIManager : MonoSingleton<UIManager>
{
	[Header("FadeScreen")]
	public GameObject fadeObj;

	[Header("FadeScreenList")]
	public GameObject screenStartCount;
	public GameObject screenWaveStart;
	public GameObject screenWaveClear;
	public GameObject screenGameClear;
	public GameObject screenGameOver;
	public GameObject screenGameResult;
	public GameObject screenTutorial;
	public GameObject screenDeath;

	[Header("DeathScreen")]
	public Text remainTimeText;

	[Header("StartPanel")]
	public GameObject startPanel;

	[Header("ReadyPanel")]
	public GameObject readyPanel;
	public Text waveClearText;

	[Header("GameClear")]
	public GameObject menuButton;
	public Text highScore;
	public Text clearScore;
	public Text addCoin;

	[Header("GameReady")]
	public GameObject playerWaitingPanel;
	public GameObject waveWaitingPanel;
	public Button weaponSelectButton;

	//Fade noraml setting
	private const float FADE_END_VALUE = 0.6f;
	private const float FADE_END_TIME = 2.0f;
	private const float FADE_DELAY = 1.5f;

	private Dictionary<string, GameObject> screenObj = new Dictionary<string, GameObject>();
	private Image fadeImg;
	private Coroutine deathCo;

	[Header("System")]
	public Image lockTouch;

	private void Start()
	{
		fadeImg = fadeObj.GetComponent<Image>();

		screenObj.Add("StartCount", screenStartCount);
		screenObj.Add("WaveStart", screenWaveStart);
		screenObj.Add("WaveClear", screenWaveClear);
		screenObj.Add("GameClear", screenGameClear);
		screenObj.Add("GameOver", screenGameOver);
		screenObj.Add("GameResult", screenGameResult);
		screenObj.Add("Tutorial", screenTutorial);
		screenObj.Add("Death", screenDeath);
	}
	public void ScreenView(string _screenName, float _fadeEndValue = FADE_END_VALUE, float _fadeEndTime = FADE_END_TIME, float _fadeDelay = FADE_DELAY)
	{
		FadeInScreen(_screenName, _fadeEndTime, 0.6f);
		CoroutineManager.Instance.CallWaitForSeconds(_fadeDelay + _fadeEndTime, () => {
		FadeOutScreen(_screenName, _fadeEndTime, 0.6f);
		});
	}
	public void OnGameOver(int _addCoin, int _score)
	{
		lockTouch.raycastTarget = true;

		if (deathCo != null) 
			StopCoroutine(deathCo);

		screenObj["Death"].SetActive(false);
		FadeInScreen("GameOver", 0.5f, 0.7f);

		CoroutineManager.Instance.CallWaitForSeconds(3.5f, () =>
		{
			screenObj["GameOver"].SetActive(false);

			OnGameResult(_addCoin, _score);

			CoroutineManager.Instance.CallWaitForSeconds(2.0f, () =>
			{
				menuButton.SetActive(true);
			});
		});
	}
	public void OnGameClear(int addCoin, int score)
	{
		lockTouch.raycastTarget = true;

		if (deathCo != null) 
			StopCoroutine(deathCo);

		screenObj["Death"].SetActive(false);
		FadeInScreen("GameClear", 0.5f, 0.7f);

		CoroutineManager.Instance.CallWaitForSeconds(3.5f, () =>
		{
			screenObj["GameClear"].SetActive(false);

			OnGameResult(addCoin, score);

			CoroutineManager.Instance.CallWaitForSeconds(2.0f, () =>
			{
				menuButton.SetActive(true);
			});
		});
	}
	public void OnGameReady()
	{
		playerWaitingPanel.SetActive(false);
		waveWaitingPanel.SetActive(true);
		weaponSelectButton.interactable = true;
	}
	public void OnGameResult(int addCoin, int score)
	{
		SoundManager.Instance.PlaySFX("GameResult");
		screenObj["GameResult"].SetActive(true);
		this.addCoin.text = addCoin.ToString();
		clearScore.text = score.ToString();
	}
	public void OnWaveClear()
	{
		if (deathCo != null) StopCoroutine(deathCo);
		screenObj["Death"].SetActive(false);

		ScreenView("WaveClear");
	}
	public void OnWaveStart(int _curWave)
	{
		lockTouch.raycastTarget = false;

		FadeInScreen("WaveStart", 1.0f, 0.6f);
		waveClearText.text = "WAVE " + _curWave;
		CoroutineManager.Instance.CallWaitForSeconds(3.0f, () =>
		{
			FadeOutScreen("WaveStart", 1.0f, 0.6f);
		});
	}
	public void OnStartScreen()
	{
		readyPanel.GetComponent<Animator>().SetBool("OnPanel", true);
		CoroutineManager.Instance.CallWaitForSeconds(1.2f, () => { 
		readyPanel.SetActive(false);
		startPanel.SetActive(true);
		startPanel.GetComponent<Animator>().SetBool("OnPanel", true);
		});
	}
	public void OnStartCountScreen()
	{
		FadeInScreen("StartCount", 0.5f, 0.6f);
		CoroutineManager.Instance.CallWaitForSeconds(10, () => {
			FadeOutScreen("StartCount", 0.5f, 0.6f);
		});
	}
	public void DeathScreenView(int _remainTime)
	{
		lockTouch.raycastTarget = true;
		FadeInScreen("Death", 0.7f, 0.6f);
		deathCo = StartCoroutine(TimerTextCo(_remainTime, remainTimeText));
		CoroutineManager.Instance.CallWaitForSeconds(_remainTime + 0.7f, () => {
			FadeOutScreen("Death", 0.7f, 0.6f);
			lockTouch.raycastTarget = false;
		});

		
	}
	public void FadeInScreen(string _screenName, float _fadeEndTime, float _fadeEndValue)
	{
		screenObj[_screenName].SetActive(true);

		Text[] t_txtList = screenObj[_screenName].GetComponentsInChildren<Text>();
		Image[] t_ImgList = screenObj[_screenName].GetComponentsInChildren<Image>();

		fadeObj.SetActive(true);

		Color initColor = fadeImg.color;
		initColor.a = 0.0f;
		fadeImg.color = initColor;
		
		fadeImg.DOFade(_fadeEndValue, _fadeEndTime);
		foreach (Text text in t_txtList)
		{
			text.DOFade(_fadeEndValue, _fadeEndTime);
		}
		foreach (Image img in t_ImgList)
		{
			img.DOFade(_fadeEndValue, _fadeEndTime);
		}
	}
	public void FadeOutScreen(string _screenName, float _fadeEndTime, float _fadeEndValue)
	{
		Text[] t_txtList = screenObj[_screenName].GetComponentsInChildren<Text>();
		Image[] t_ImgList = screenObj[_screenName].GetComponentsInChildren<Image>();

		fadeImg.GetComponent<Image>().DOFade(0f, _fadeEndTime).OnComplete(() => {
			fadeObj.SetActive(false);
			screenObj[_screenName].SetActive(false);
		});
		foreach (Text text in t_txtList)
		{
			text.DOFade(0.0f, _fadeEndTime);
		}
		foreach (Image img in t_ImgList)
		{
			img.DOFade(0.0f, _fadeEndTime);
		}
	}

	[PunRPC]
	public void RpcFadeOut()
	{
		FadeOutScreen("screenStartCount", 0.5f, 0.6f);
	}
	public IEnumerator TimerTextCo(int _remainTime, Text _text)
	{
		int t_remainTime = _remainTime;

		while(t_remainTime >= 0)
		{
			_text.text = t_remainTime.ToString();
			yield return new WaitForSeconds(1.0f);
			t_remainTime--;
		}
	}
}