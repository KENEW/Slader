using System.Collections;
using UnityEngine;
using Photon.Pun;
using static SladerGame;
using Random = UnityEngine.Random;
public enum PlayerState
{
    Live,
    Death
}
public enum MoveState
{
    Idle,
    Walk,
    Dash
}
public class Player : MonoBehaviourPunCallbacks
{
    private const int WALK_SPEED = 70;

    public int playerNum = 0;

    public LiftWeapon liftWeapon;
    public GameObject NoFightImg;
    public PlayerHp playerHp;

    private AttackButton attackButton;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public GameObject ArrowPre;

    [Header("PoolName")]
    [SerializeField] private string swordEffectPoolName;
    [SerializeField] private string attSpEfPoolName;

    private Vector2 spawnPos;

    public PlayerState state = PlayerState.Live;
    public MoveState moveState = MoveState.Walk;

    private bool isFlip = false;
    private bool isDamageDelay = false;

    private float keyX = 0, keyY = 0;
    private int moveSpeed = 0;
    private int curWeapon = 0;
    private int offensiveAttack = 10;

    private int manaUpgradeLevel = 1;
    private int attackUpgradeLevel = 1;
    private int coinUpgradeLevel = 1;

    public bool isMove = false;
    public bool IsDash = false;

    #region UNITY
    private void Awake()
    {
        attackButton = FindObjectOfType<AttackButton>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
	private void Start()
	{
        offensiveAttack = MyData.Instance.weaponInfo[0].offensiveAttack;
    }
	private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (!isMove)
        {
            OnMove(false);
        }

#if UNITY_EDITOR
        if (state == PlayerState.Live)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W))
            {
                keyX = Input.GetAxis("Horizontal");
                keyY = Input.GetAxis("Vertical");
                OnMove(true, keyX, keyY);

                isMove = true;
            }
            else
            {
                isMove = false;
            }
        }
#endif
    }
    private void FixedUpdate()
    {
        switch (moveState)
        {
            case MoveState.Idle:
                rb.velocity = new Vector2(0, 0);
                break;
            case MoveState.Walk:
                rb.velocity = (new Vector2(keyX, keyY)) * moveSpeed * Time.deltaTime;
                break;
            case MoveState.Dash:
                rb.velocity = (new Vector2(keyX, keyY)) * (moveSpeed * 1.4f) * Time.deltaTime;
                break;
        }
    }
	public void OnMove(bool isMove, float moveX = 0, float moveY = 0)
	{
		keyX = moveX;
		keyY = moveY;

		if (!isMove || state == PlayerState.Death)
		{
			animator.SetBool("Working", false);
			moveState = MoveState.Idle;
			moveSpeed = 0;
		}
		else
		{
			animator.SetBool("Working", true);
            moveSpeed = WALK_SPEED;
            moveState = IsDash ? MoveState.Dash : MoveState.Walk;
        }
	}
	public void OnAttack(Vector2 dir)
    {
        var call = DamageCall(GetOffensiveAttack());
        int damage = call.Item1;
        bool isCritical = call.Item2;

        switch (curWeapon)
        {
            case 0:
                {
                    photonView.RPC(nameof(OnSword), RpcTarget.All, dir, damage, isCritical);
                }
                break;
            case 1:
                {
                    photonView.RPC(nameof(OnArrow), RpcTarget.All, dir, damage, isCritical);
                }
                break;
            case 2:
                {
                    photonView.RPC(nameof(OnFireball), RpcTarget.All, dir, damage, isCritical);
                }
                break;
        }
    }
    public void OnSpawn()
    {
        transform.position = spawnPos;

        GameObject spawnEf = ObjectManager.Instance.effectPool.ObjectDequeue("MonsterSpawnEffect");
        spawnEf.transform.position = this.transform.localPosition;
    }
    public void SetWeapon(int weaponIndex)
    {
        curWeapon = weaponIndex;
        liftWeapon.WeaponChange(weaponIndex);
        offensiveAttack = MyData.Instance.weaponInfo[weaponIndex].offensiveAttack;

        GameObject weaponChangeEf = ObjectManager.Instance.effectPool.ObjectDequeue("WeaponChangeEffect");
        weaponChangeEf.transform.position = transform.position;

        if (photonView.IsMine)
        {
            AttackButton.Instance.SetAttackSpeed(weaponIndex);
        }
    }
    private (int, bool) DamageCall(int weaponDamage)
    {
        bool isCritical = (20 >= Random.Range(0, 100));
        int damage = (int)Random.Range(weaponDamage * 0.9f, weaponDamage * 1.1f);
        damage = isCritical ? (int)(damage * 1.5f) : damage;
        return (damage, isCritical);
    }
    public int GetOffensiveAttack()
    {
        return (int)(offensiveAttack + ((offensiveAttack * 0.02f) * attackUpgradeLevel));
    }
    public int GetManaSoul()
    {
        return (int)(NORMAL_USER_MANA + (NORMAL_USER_MANA * 0.02f) * manaUpgradeLevel);
    }
    #endregion

    #region PUN_CALLBACK
    [PunRPC]
    public void GetDamage(int damage)
    {
        if (!isDamageDelay)
        {
            SoundManager.Instance.PlaySFX("PlayerHurt");

            CameraController.Instance.OnShake(playerNum, 1.0f, 8.5f);
            playerHp.GetDamage(damage);
            //StartCoroutine(Invincibility());
        }
    }
    [PunRPC]
    public void SetUpgradeInfo(int manaLevel, int attackLevel, int coinLevel)
    {
        manaUpgradeLevel = manaLevel;
        attackUpgradeLevel = attackLevel;
        coinUpgradeLevel = coinLevel;
    }
    [PunRPC]
    public void Init(int index, Vector2 spawnPos)
    {
        playerNum = index;
        playerHp = PlayerManager.Instance.userHpBar[index].GetComponent<PlayerHp>();
        gameObject.name = "Player_" + index;
        spriteRenderer.sortingOrder = 1000 + playerNum;
        this.spawnPos = spawnPos;
        liftWeapon.SetPlayer(index);
        OnSpawn();
    }
    [PunRPC]
    private void OnArrow(Vector2 dir, int damage, bool isCritical)
    {
        SoundManager.Instance.PlaySFX("RelasingBow");

        GameObject t_arrow = ObjectManager.Instance.projectilePool.ObjectDequeue("PlayerArrow");
        float t_angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90;
        t_arrow.transform.position = transform.position;

        t_arrow.GetComponent<PlayerArrow>().GetInfo(damage, isCritical, Vector2.up, 9.0f, 3, t_angle, 0f);
    }
    [PunRPC]
    private void OnFireball(Vector2 dir, int damage, bool isCritical)
    {
        SoundManager.Instance.PlaySFX("FireBall");

        GameObject fireBall = ObjectManager.Instance.projectilePool.ObjectDequeue("FireBall");
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + 90 + 180;
        fireBall.transform.position = transform.position;

        fireBall.GetComponent<FireBall>().GetInfo(damage, isCritical, Vector2.up, 6.0f, 100, angle, 0.0f);
    }
    [PunRPC]
    public void OnSword(Vector2 dir, int damage, bool isCritical)
    {
        SoundManager.Instance.PlaySFX("SwordAttack");

        GameObject effect = ObjectManager.Instance.effectPool.ObjectDequeue("SwordEffect");

        Vector3 flipScale = new Vector3(isFlip ? -1 : 1, 1, 1);
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        effect.transform.rotation = Quaternion.Euler(0, 0, angle - 75);
        effect.transform.position = transform.position;
        effect.transform.localScale = flipScale;

        if (liftWeapon.ContinuousAttack)
        {
            effect.transform.localScale = new Vector3(flipScale.x, flipScale.y, flipScale.z);
        }
        else
        {
            effect.transform.localScale = new Vector3(-flipScale.z, flipScale.y, flipScale.z);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Collider2D[] enemys = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (dir.x * 0.7f),
                transform.position.y + (dir.y * 0.7f)), new Vector3(0.8f, 0.8f),
                Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f);

            foreach (Collider2D i in enemys)
            {
                if (i.transform.CompareTag("Enemy"))
                {
                    i.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage, isCritical);
                }
                if (i.transform.CompareTag("Interaction"))
                {
                    i.GetComponent<PhotonView>().RPC("OnInteraction", RpcTarget.All);
                }
            }
        }
    }
    #endregion

    #region COROUTINE
    /// <summary>
    /// 플레이어가 데미지를 입을 시 일정시간동안 무적판정을 받으며 보호된다.
    /// 비주얼 -> 일정시간마다 캐릭터가 깜빡거린다.
    /// </summary>
    /// <returns></returns>
    IEnumerator Invincibility()
    {
        int countTime = 0;

        isDamageDelay = true;
        while (countTime < 2)
        {
            if (countTime % 2 == 0)
            {
                spriteRenderer.color = new Color32(255, 255, 255, 90);
            }
            else
            {
                spriteRenderer.color = new Color32(255, 255, 255, 180);
            }

            yield return new WaitForSeconds(0.05f);
            countTime++;
        }
        spriteRenderer.color = new Color32(255, 255, 255, 255);
        isDamageDelay = false;
    }
    #endregion
}