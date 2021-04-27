using UnityEngine;

public class CoinBubbleManager : MonoSingleton<CoinBubbleManager>
{
	public GameObject coinBubble;
	public GameObject coinBubbleText;

	public Transform parentTrans;

	public Transform particlePos;
	public ParticleSystem particleSys;

	public void PlayPraticleEffect(Vector3 pos)
	{
		particlePos.position = pos;
		particleSys.Play();
	}
	public void CreateCoinBubble(int y, int x)
	{
		Vector3 pos = (new Vector3(x * 0.6f, -(y * 0.6f) , 1) - new Vector3(1.8f, -3.05f, 0));
		GameObject obj = Instantiate(coinBubble, parentTrans);
		obj.GetComponent<RectTransform>().position = pos;
	}
	public void CreateCoinBubbleText(Vector3 pos)
	{
		Instantiate(coinBubbleText, pos, Quaternion.identity, parentTrans);
	}
}
