using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum UpgradeType
{
	manaSoul,
	offensiveAttack,
	coin
}
public class UpgradePanel : MonoSingleton<UpgradePanel>
{
	private UpgradeType? curUpgradeType = null;
	public Transform[] upgradeTypeButtonPos;
	public GameObject upgradeTypeSelect;
	public Button upgradeButton;
	public GameObject upgradePanel;
	public GameObject maskPanel;
	public Text coinText;

	[Header("CurrentLevelText")]
	public Text manaSoulLevelText;
	public Text offensiveAttackLevelText;
	public Text coinLevelText;

	[Header("UpgradeInfoText")]
	public Text upgradeInfoText;
	public Text existingText;
	public Text increaseText;

	public Text upgradeCurLevel;
	public Text upgradeNextLevel;
	public Text necessaryCoinText;

	[Header("NoticePanel")]
	public GameObject upgradeMinPanel;
	public GameObject upgradeMaxPanel;
	public GameObject upgradeNullPanel;

	private CharData charData;
	private UpgradeCost[] upgradeCost;

	private Vector2 cameraHeightSize;

	public readonly string[] UPGRADE_INFO_STR =
	{
		"최대 마나가 상승합니다.",
		"추가 공격력을 얻습니다.",
		"코인 획득량이 상승합니다."
	};

	public GameObject upgradeCanvas;

	private void Start()
	{
		upgradeCost = MyData.Instance.upgradeCost;
		charData = MyData.Instance.charData;
		cameraHeightSize = Camera.main.WorldToScreenPoint(new Vector2(0, 2 * Camera.main.orthographicSize));
	}
	public void SetUpgradeType(int upgradeType)
	{
		SoundManager.Instance.PlaySFX("ButtonClick");

		curUpgradeType = (UpgradeType)upgradeType;

		if (curUpgradeType != null)
		{
			upgradeTypeSelect.SetActive(true);
			upgradeTypeSelect.transform.position = new Vector2(upgradeTypeButtonPos[upgradeType].position.x, upgradeTypeSelect.transform.position.y);
		}

		UpgradeButtonUIUpdate();
	}
	public void UpgradeButton()
	{
		switch (curUpgradeType)
		{
			case UpgradeType.manaSoul:
				if (charData.manaLevel < 40)
				{
					if (MyData.Instance.UseMyCoin(upgradeCost[charData.manaLevel].manaSoulCost))
					{
						charData.manaLevel++;
					}
					else
					{
						upgradeButton.interactable = false;
					}
				}
				break;
			case UpgradeType.offensiveAttack:
				if (charData.attackLevel < 40)
				{
					if (MyData.Instance.UseMyCoin(upgradeCost[charData.attackLevel].manaSoulCost))
					{
						charData.attackLevel++;
					}
					else
					{
						upgradeButton.interactable = false;
					}
				}
				break;
			case UpgradeType.coin:
				if (charData.coinLevel < 40)
				{
					if (MyData.Instance.UseMyCoin(upgradeCost[charData.coinLevel].manaSoulCost))
					{
						charData.coinLevel++;
					}
					else
					{
						upgradeButton.interactable = false;
					}
				}
				break;
		}

		SoundManager.Instance.PlaySFX("CoinSpend");

		ServerData.Instance.SaveData();
		DataController.Instance.SaveGameData();

		UIUpdate();
	}
	private void UpgradeButtonUIUpdate()
	{
		int cost = 0;
		int curLevel = 0;
		int increaseNum = 0;
		int existingNum = 0;
		bool maxLevelCheck = false;

		switch (curUpgradeType)
		{
			case null:
				upgradeMinPanel.SetActive(false);
				upgradeNullPanel.SetActive(false);
				upgradeMaxPanel.SetActive(false);
				upgradeButton.interactable = false;

				manaSoulLevelText.text = charData.manaLevel < 40 ? charData.manaLevel + "" : "Max";
				offensiveAttackLevelText.text = charData.attackLevel < 40 ? charData.attackLevel + "" : "Max";
				coinLevelText.text = charData.coinLevel < 40 ? charData.coinLevel + "" : "Max";
				return;
			case UpgradeType.manaSoul:
				if (charData.manaLevel >= 40)
				{
					maxLevelCheck = true;
				}
				else
				{
					maxLevelCheck = false;
				}
				curLevel = charData.manaLevel;
				existingNum = charData.manaLevel;
				increaseNum = 1;
				cost = maxLevelCheck ? 0 : upgradeCost[charData.manaLevel].manaSoulCost;
				break;
			case UpgradeType.offensiveAttack:
				if (charData.attackLevel >= 40)
				{
					maxLevelCheck = true;
				}
				else
				{
					maxLevelCheck = false;
				}
				curLevel = charData.attackLevel;
				existingNum = charData.attackLevel;
				increaseNum = 1;
				cost = maxLevelCheck ? 0 : upgradeCost[charData.attackLevel].attackCost;
				break;
			case UpgradeType.coin:
				if (charData.coinLevel >= 40)
				{
					maxLevelCheck = true;
				}
				else
				{
					maxLevelCheck = false;
				}
				curLevel = charData.coinLevel;
				existingNum = charData.coinLevel;
				increaseNum = 1;
				cost = maxLevelCheck ? 0 : upgradeCost[charData.coinLevel].coinCost;
				break;
		}

		upgradeNullPanel.SetActive(true);

		manaSoulLevelText.text = charData.manaLevel < 40 ? charData.manaLevel + "" : "Max";
		offensiveAttackLevelText.text = charData.attackLevel < 40 ? charData.attackLevel + "" : "Max";
		coinLevelText.text = charData.coinLevel < 40 ? charData.coinLevel + "" : "Max";

		necessaryCoinText.text = cost + "";
		necessaryCoinText.color = MyData.Instance.MyCoin < cost ? Color.red : Color.white;
		upgradeCurLevel.text = curLevel + "";
		upgradeNextLevel.text = curLevel <= 38 ? curLevel + 1 + "" : "Max";
		upgradeInfoText.text = UPGRADE_INFO_STR[(int)curUpgradeType];
		existingText.text = existingNum + "%";

		upgradeMinPanel.SetActive(curLevel > 39 ? false : true);
		upgradeMaxPanel.SetActive(curLevel > 39 ? true : false);

		upgradeButton.interactable = maxLevelCheck ? false : true;

		if (increaseNum > 0)
		{
			existingText.text += " <color=#5F8ACF>(+" + increaseNum;
			existingText.text += "%";
			existingText.text += ")</color>";
		}
	}
	public void OpenUpgradePanel()
	{
		SoundManager.Instance.PlaySFX("ButtonClick");

		upgradeCanvas.SetActive(true);
		upgradeTypeSelect.SetActive(false);
		curUpgradeType = null;

		UIUpdate();
		
		upgradePanel.transform.DOMove(new Vector3(upgradePanel.transform.position.x, 0, 0), 0.5f).SetEase(Ease.InOutCubic);
		maskPanel.SetActive(true);
	}
	public void CloseUpgradePanel()
	{
		SoundManager.Instance.PlaySFX("ButtonClick");

		upgradeCanvas.SetActive(false);
		upgradePanel.transform.DOMoveY(-cameraHeightSize.y, 0.5f).SetEase(Ease.InOutCubic);
		maskPanel.SetActive(false);
	}
	public void UIUpdate()
	{
		coinText.text = MyData.Instance.charData.coin.ToString();
		UpgradeButtonUIUpdate();
	}
}