using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class LoadScene : MonoSingleton<LoadScene>
{
	public GameObject loacCanvas;
	public GameObject loadingPanel;
	public Image curLoadGuageBar;
	public Image fadeScreen;

	public void LoadStart(string sceneName)
	{
		loacCanvas.SetActive(true);
		fadeScreen.DOFade(1, 2.0f).OnComplete(() =>
		{
			loadingPanel.SetActive(true);
			fadeScreen.DOFade(0, 2.0f).OnComplete(() =>
			{
				StartCoroutine(LoadingCo(sceneName));
			});
		});
	}
	private IEnumerator LoadingCo(string sceneName)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

		operation.allowSceneActivation = false;

		float timer = 0;

		while (!operation.isDone)
		{
			if(operation.progress < 0.9f)
			{ 
				curLoadGuageBar.fillAmount = operation.progress;
			}
			else
			{
				timer += Time.deltaTime;
				curLoadGuageBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

				if (curLoadGuageBar.fillAmount >= 1.0f)
				{
					fadeScreen.DOFade(1, 1.0f).OnComplete(() => 
					{
						DOTween.KillAll();
						//operation.allowSceneActivation = true; 
						PhotonNetwork.LoadLevel(sceneName);
					});
					yield break;
				}
			}

			yield return null;
		}
		if (PhotonNetwork.LevelLoadingProgress > 0 && PhotonNetwork.LevelLoadingProgress < 1)
		{
			DebugOptimum.Log(PhotonNetwork.LevelLoadingProgress);
		}
	}
}
