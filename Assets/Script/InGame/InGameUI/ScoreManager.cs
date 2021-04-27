using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    public Text scoreText;

    public Image scoreGauge;

    public Animator animator;

    private int curScore = 0;

    public void ScoreInit()
    {
        ScoreAdd(0);
    }
    public void ScoreAdd(int scoreValue)
    {
        curScore += scoreValue;

        if (curScore >= MyData.Instance.charData.highScore)
        {
            MyData.Instance.charData.highScore = curScore;
        }

        scoreGauge.fillAmount = ((float)curScore / (float)MyData.Instance.charData.highScore);
        animator.SetTrigger("ScoreZoom");

        scoreText.text = curScore + "";
    }
    public int GetScore()
    {
        return curScore;
    }
}