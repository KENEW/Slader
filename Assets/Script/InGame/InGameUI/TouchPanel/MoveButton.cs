using UnityEngine;
using UnityEngine.EventSystems;

public class MoveButton : MonoBehaviour, IDragHandler, IPointerUpHandler
{
    public Joystick joyStick;
    public Player player;

    private bool isState = false;

    private void Start()
    {
        player = PlayerManager.Instance.GetPlayer();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (PlayerManager.Instance.GetPlayer().state == PlayerState.Death)
        {
            player.OnMove(false);
            player.isMove = false;
		}
        else
        {
            player.OnMove(true, joyStick.Direction.x, joyStick.Direction.y);
            player.isMove = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        player.OnMove(false);
        player.isMove = false;
    }
}