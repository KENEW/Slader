using UnityEngine.UI;

public class CoinManager : MonoSingleton<CoinManager>
{
    public Text coinText;
    
    public void CoinLoad()
    {
        UIUpdate();
    }
    public int GetCoin(int value)
    {
        return MyData.Instance.MyCoin;
    }
    public void AddCoin(int value)
    {
        MyData.Instance.charData.coin += value;
        UIUpdate();
    }
    private void UIUpdate()
    {
        coinText.text = MyData.Instance.MyCoin + "";
    }
}