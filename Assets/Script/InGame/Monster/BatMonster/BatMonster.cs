using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class BatMonster : Monster
{
	private readonly float ATTACK_READY_TIME = 0.5f;
	private readonly int ATTACK_DAMAGE = 40;

	public GameObject bombZone;

	protected override void Awake()
	{
		base.Awake();

		maxHp = 150;
		moveSpeed = 1.6f;
		dropCoinValue = 100;

		SetCoopDifficulty();
	}
	protected override void Start()
	{
		base.Start();

		//소환된 후 일정시간 후에 생존하는 플레이어를 검색하고 찾으면 몬스터 상태를 시작한다.
		if (PhotonNetwork.IsMasterClient)
		{
			CoroutineManager.Instance.CallWaitForSeconds(1.4f, () =>
			{
				StartCoroutine(FindPlayerNear(5.0f));
				photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Idle);
				isStateStart = true;
			});
		}
	}
	protected override void Update()
	{
		base.Update();
		
		if (!PhotonNetwork.IsMasterClient || !isStateStart || !player)
		{
			return;
		}

		if (player.state == PlayerState.Death)
		{
			if (!isSearch)
			{
				photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Idle);
				StartCoroutine(PlayerStateSearch(PlayerState.Live));
				isSearch = true;
			}
			return;
		}

		MonsterStateUpdate();
	}
	private void MonsterStateUpdate()
	{
		switch (state)
		{
			case State.Idle:
				if (PlayerDistanceCheck(6.0f))
				{
					photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Trace);
				}
				break;
			case State.Trace:
				if (!PlayerDistanceCheck(6.0f))
				{
					photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Idle);
				}
				if (PlayerDistanceCheck(0.45f))
				{
					photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Attack);
				}
				break;
			case State.Attack:
				break;
		}
	}
	[PunRPC]
	private void ChangeState(State state)
	{
		switch (this.state)
		{
			case State.Idle:
				break;
			case State.Trace:
				break;
			case State.Attack:
				break;
		}

		this.state = state;

		switch (state)
		{
			case State.Idle:
				aiPath.canMove = true;
				SetTarget(Target);
				break;
			case State.Trace:
				aiPath.canMove = true;
				SetTarget(player.transform);
				bombZone.SetActive(false);
				break;
			case State.Attack:
				isAttack = true;
				aiPath.canMove = false;

				bombZone.SetActive(true);
				bombZone.GetComponent<BatBomb>().SetInit(ATTACK_READY_TIME, ATTACK_DAMAGE);
				StartCoroutine(ChargingStartCo(ATTACK_READY_TIME, () =>
				{
					bombZone.GetComponent<BatBomb>().OnBomb();
				}));

				transform.DOScale(1.2f, ATTACK_READY_TIME);
				spriteRender.DOColor(Color.red, ATTACK_READY_TIME);
				break;
		}
	}
}
