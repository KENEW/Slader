using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class AttackButton : MonoSingleton<AttackButton>, IPointerDownHandler, IPointerUpHandler
{
	public Joystick joystick;
	public Castingbar castingBar;

	private Player player;
	private LiftWeapon liftWeapon;

	private float pushTime = 0f;
	private float attackSpeed = 1.0f;
	private bool pushCheck = false;
	private bool castingCheck = false;

	private bool stateCheck = false;

	private void Start()
	{
		player = PlayerManager.Instance.GetPlayer();
		liftWeapon = FindObjectOfType<LiftWeapon>();
		castingBar = FindObjectOfType<Castingbar>();

		SetAttackSpeed(0);
	}
	public void Update()
	{
		if (PlayerManager.Instance.GetPlayer().state == PlayerState.Death)
		{
			if (!stateCheck)
			{
				PushInit();
				stateCheck = true;
			}
			return;
		}
		else
		{
			stateCheck = false;
		}

		if (pushCheck)
		{
			pushTime += Time.deltaTime;

			if (castingCheck)
			{
				liftWeapon.LiftWeaponReady();
				castingBar.CastingStart(attackSpeed);
				castingCheck = false;
			}
			if (pushTime >= attackSpeed)
			{
				liftWeapon.LiftWeaponAttack();
				player.OnAttack(joystick.Direction);
				castingBar.CastingEnd();
				castingCheck = true;
				pushTime = 0;
			}
		}
	}
	public void OnPointerDown(PointerEventData ped)
	{ 
		pushCheck = true;
	}
	public void OnPointerUp(PointerEventData ped)
	{
		PushInit();
	}
	public void SetAttackSpeed(int weaponIndex)
	{
		attackSpeed = MyData.Instance.weaponInfo[weaponIndex].attackSpeed;
	}
	private void PushInit()
	{
		pushTime = 0;
		pushCheck = false;
		castingCheck = true;
		castingBar.CastingEnd();
	}
}