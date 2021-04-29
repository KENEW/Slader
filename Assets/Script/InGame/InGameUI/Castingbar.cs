using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 다음 공격 딜레이를 시각적으로 표시해준다.
/// </summary>
public class Castingbar : MonoBehaviour
{
	public GameObject castingBarObj;
	private Image castingbar;
	
	private float curTime;
	private float destTime;

	private void Start()
	{
		castingbar = castingBarObj.transform.GetChild(0).GetComponent<Image>();
	}
	public void CastingStart(float destTime)
	{
		this.destTime = destTime;
		castingBarObj.SetActive(true);
		StartCoroutine(nameof(CastingBarCo));
	}
	public void CastingEnd()
	{
		StopCoroutine(nameof(CastingBarCo));
		curTime = 0.0f;
		castingbar.fillAmount = curTime / destTime;
		castingBarObj.SetActive(false);
	}
	IEnumerator CastingBarCo()
	{
		while (curTime <= destTime)
		{
			curTime += Time.deltaTime;
			castingbar.fillAmount = curTime / destTime;
			yield return null;
		}

		CastingEnd();
	}
}