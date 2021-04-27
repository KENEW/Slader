using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : MonoBehaviour
{
    public GameObject statisticsPanel;
    public Text acumulatedMonsterNum;
    public Text highScoreNum;
    public Text highWaveNum;

    public void OpenPanel()
    {
        SoundManager.Instance.PlaySFX("ButtonClick");
        statisticsPanel.SetActive(true);
        UIUpdate();
    }
    public void ClosePanel()
    {
        SoundManager.Instance.PlaySFX("ButtonClick");
        statisticsPanel.SetActive(false);
    }
    private void UIUpdate()
    {
        acumulatedMonsterNum.text = MyData.Instance.charData.gameCount.ToString();
        highScoreNum.text = MyData.Instance.charData.highScore.ToString();
        highWaveNum.text = MyData.Instance.charData.highWave.ToString();
    }
}
