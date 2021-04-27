using UnityEngine;
using Photon.Pun;

public class SkeletonMonster : Monster
{
	private readonly float ATTACK_DELAY = 1.3f;
	private readonly int ATTACK_DAMAGE = 20;

	public Animator animator;

	protected override void Awake()
	{
		base.Awake();

		maxHp = 300;
		moveSpeed = 0.6f;
		dropCoinValue = 130;

		SetCoopDifficulty();
	}
	protected override void Start()
	{
		base.Start();

		animator = GetComponent<Animator>();
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

		if(player.state == PlayerState.Death)
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
	public void MonsterStateUpdate()
	{
		switch (state)
		{
			case State.Idle:
				if (PlayerDistanceCheck(5.0f))
				{
					photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Trace);
				}
				break;
			case State.Trace:
				if (!PlayerDistanceCheck(5.0f))
				{
					photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Idle);
				}
				if (PlayerDistanceCheck(4.2f))
				{
					photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Attack);
				}
				break;
			case State.Attack:
				break;
		}
	}
	[PunRPC]
	public void ChangeState(State state)
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
				animator.SetBool("Idle", true);
				aiPath.canMove = true;
				SetTarget(Target);
				break;
			case State.Trace:
				animator.SetBool("Idle", true);
				aiPath.canMove = true;
				SetTarget(player.transform);
				break;
			case State.Attack:
				animator.SetBool("Idle", false);
				aiPath.canMove = false;
				isAttack = true;

				StartCoroutine(ChargingStartCo(ATTACK_DELAY, () =>
				{
					if (PhotonNetwork.IsMasterClient)
					{
						if (isAttack)
						{
							photonView.RPC(nameof(OnArrow), RpcTarget.All);
							isAttack = false;
						}

						CoroutineManager.Instance.CallWaitForSeconds(2.0f, () =>
						{
							photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Idle);
						});
					}
				}));
				break;
		}
	}
	[PunRPC]
	public void OnArrow()
	{
		GameObject arrow = ObjectManager.Instance.projectilePool.ObjectDequeue("SkeletonArrow");

		arrow.transform.position = transform.position;
		arrow.GetComponent<Arrow>().GetInfo(ATTACK_DAMAGE, false, Vector2.up, 5.5f, 1, GetAngleBetweenPlayer(), 0, 2.5f);
	}
}