using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DashButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public Player player;
	private bool pushCheck = false;

	private void Start()
	{
		player = PlayerManager.Instance.GetPlayer();
	}
	private void Update()
	{
		//if(pushCheck)
		//{
		//	player.OnMoveChange(true);
		//}
	}
	public void OnPointerDown(PointerEventData ped)
	{
		//pushCheck = true;
		player.IsDash = true;
	}
	public void OnPointerUp(PointerEventData ped)
	{
		//pushCheck = false;
		//player.OnMoveChange(false);
		player.IsDash = false;
	}
}
