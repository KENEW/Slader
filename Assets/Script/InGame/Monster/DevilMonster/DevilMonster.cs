using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DevilMonster : Monster
{
	private const float ATTACK_READY_TIME = 1.1f;
	private const int DASH_SATTACK_DAMAGEPEED = 20;
	private const int DASH_SPEED = 400;

	public Animator animator;

	private float attackReadyTime = 0.0f;
	private float attackDelay = 0.4f;
	private float dashSpeed = 0;
	private bool attackCheck = false;
	private bool collCheck = false;
	private bool isCharging = false;

	private Vector2 attackDir = Vector2.zero;
	
	private PlayerCollCheck playerColl;

	protected override void Awake()
	{
		base.Awake();

		maxHp = 400;
		moveSpeed = 0.65f;
		dropCoinValue = 170;

		SetCoopDifficulty();
	}
	protected override void Start()
	{
		base.Start();

		playerColl = new PlayerCollCheck(FindObjectsOfType<Player>());
		animator = GetComponent<Animator>();

		//소환된 후 일정시간 후에 생존하는 플레이어를 검색하고 찾으면 몬스터 상태를 시작한다.
		if (PhotonNetwork.IsMasterClient)
		{
			StartCoroutine(FindPlayerNear(5.0f));
			CoroutineManager.Instance.CallWaitForSeconds(1.4f, () =>
			{
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

	

		MonsterStateUpdate();
	}
	private void MonsterStateUpdate()
	{
		switch (state)
		{
			case State.Idle:
				if(player.state == PlayerState.Death)
				{
					if (!isSearch)
					{
						StartCoroutine(PlayerStateSearch(PlayerState.Live));
						isSearch = true;
					}
					return;
				}

				if (PlayerDistanceCheck(4.0f))
				{
					photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Trace);
				}
				break;
			case State.Trace:
				if(player.state == PlayerState.Death)
				{

						photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Idle);
					
					return;
				}

				if (!PlayerDistanceCheck(4.0f))
				{
					photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Idle);
				}
				if (PlayerDistanceCheck(1.5f))
				{
					photonView.RPC(nameof(ChangeState), RpcTarget.All, State.Attack);
				}
				break;
			case State.Attack:
				if (attackCheck)
				{
					dashSpeed -= dashSpeed * (Time.deltaTime * 2.4f);

					//대쉬량이 일정 이상일 때 플레이어 충돌 판정이 생긴다.
					if (dashSpeed > DASH_SPEED * 0.3f)
					{
						PlayerCollCheck();
					}
					//대쉬량이 일정량 미만이 될 경우 충돌 판정을 종료한다.
					if (dashSpeed < 1)
					{
						attackCheck = false;
						StartCoroutine(SlimeAttackTime(attackDelay,() => 
						{ 
							ChangeState(State.Idle); 
						}));
					}
					else
					{
						rigid.velocity = attackDir * dashSpeed * Time.deltaTime;
					}
				}
				break;
		}
	}
	[PunRPC]
	public void ChangeState(State state)
	{
		isSearch = false;
		
		switch (this.state)
		{
			case State.Idle:
				break;
			case State.Trace:
				break;
			case State.Attack:
				attackReadyTime = 0.0f;
				flipLock = false;
				break;
		}

		this.state = state;

		switch (state)
		{
			case State.Idle:
				aiPath.canMove = true;
				SetTarget(Target);
				animator.SetBool("Idle", true);
				break;
			case State.Trace:
				aiPath.canMove = true;
				SetTarget(player.transform);
				animator.SetBool("Idle", true);
				break;
			case State.Attack:
				isAttack = true;
				dashSpeed = DASH_SPEED;
				playerColl.Init();
				animator.SetBool("Idle", false);

				StartCoroutine(ChargingStartCo(ATTACK_READY_TIME, () =>
				{
					isCharging = true;
					attackCheck = true;
					attackDir = (player.transform.position - transform.position).normalized;

					isFlip = player.transform.position.x < transform.position.x ? true : false;
					animator.SetTrigger("Attack");
				}));

				attackCheck = false;
				aiPath.canMove = false;
				flipLock = true;
				break;
		}
	}
	private void PlayerCollCheck()
	{
		Collider2D[] t_moveCheck = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.15f, 0.15f), 0f);

		foreach (Collider2D i in t_moveCheck)
		{
			if (i.transform.CompareTag("Player"))
			{
				if (i.transform.TryGetComponent<Player>(out Player player))
				{
					if (playerColl.CheckPlayer(player.playerNum))
					{
						i.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.All, 25);
						playerColl.OnColl(player.playerNum);
					}
				}
			}
		}
	}
	IEnumerator SlimeAttackTime(float t, Action act)
	{
		yield return new WaitForSeconds(t);
		act();
	}
}
