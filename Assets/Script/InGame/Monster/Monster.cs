using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

using Photon.Pun;
using static SladerGame;
using Random = UnityEngine.Random;

public enum State
{
    Idle,
    Trace,
    Attack
}
public class Monster : Enemy, IPunObservable
{
    private const float HP_INCREASE = 1.4f;

    //Ai - Fathfinder
    protected AIDestinationSetter aiDest;
    protected AIPath aiPath;

    //Attack charging gauge bar
    protected GameObject attChBar;
    protected Image attChBarImg;

    public Player player;
    public Player[] players;
    public Transform Target;

    [SerializeField] protected State state = State.Idle;

    protected bool isKillDirectly = true;
    protected bool isSearch = false;
    protected bool isStateStart = false;
    protected bool isAttack = false;

    protected float playerDis = 0.0f;
    protected bool flipLock = false;
    protected bool isFlip = false;

    protected float moveSpeed = 1.0f;
    protected int dropCoinValue;

    #region UNITY

    protected virtual void Awake()
    {
        aiDest = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }

    protected virtual void Start()
    {
        attChBar = ObjectManager.Instance.uIPool.ObjectDequeue("AttackChargingBar");
        attChBarImg = attChBar.transform.GetChild(0).GetComponent<Image>();
        attChBar.SetActive(false);

        players = FindObjectsOfType<Player>();

        Invoke(nameof(AISearch), 1.3f);

        SetTarget(ObjectManager.Instance.doorTarget.transform);
        Target = ObjectManager.Instance.doorTarget.transform;
    }

    protected override void Update()
    {
        base.Update();

        attChBar.transform.position = Camera.main.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y - 0.5f));
        spriteRender.flipX = isFlip ? true : false;

        if (PhotonNetwork.IsMasterClient)
        {
            if (!flipLock)
            {
                OnFlipping(aiDest.target);
            }
        }
    }

    public override void OnDisable()
    {
        if (isActive)
        {
            ObjectManager.Instance.uIPool.ObjectEnqueue("AttackChargingBar", attChBar);
            ObjectManager.Instance.uIPool.ObjectEnqueue("MonsterHpBar", hpBar);
        }

        base.OnDisable();
    }

    protected void SetCoopDifficulty()
    {
        if(!NetworkManager.Instance.IsCoop)
        {
            return;
        }

        maxHp = (int)(maxHp * HP_INCREASE);
    }

    protected void PlayerSearch()
    {
        var neareastPlayer = players.OrderBy(obj =>
        {
            return Vector2.Distance(transform.position, obj.transform.position);
        })
        .Where(obj =>
        { 
            return obj.state == PlayerState.Live; 
        })
        .FirstOrDefault();

        if (neareastPlayer != null)
        {
            photonView.RPC(nameof(GetPlayerRPC), RpcTarget.All, neareastPlayer.playerNum);
            isSearch = false;
        }
    }

    protected void AISearch()
    {
        if (gameObject.activeSelf)
        {
            aiPath.canSearch = true;
            aiPath.canMove = true;
            aiPath.maxSpeed = moveSpeed;
        }
    }

    protected void OnFlipping(Transform targetTransform)
    {
        if (transform.position.x > targetTransform.position.x)
        {
            isFlip = true;
        }
        else
        {
            isFlip = false;
        }
    }

    protected float GetAngleBetweenPlayer()
    {
        Vector2 dir = transform.position - player.transform.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
    }

    protected bool PlayerDistanceCheck(float distance)
    {
        playerDis = Vector2.Distance(transform.position, player.transform.position);
        return (playerDis <= distance) ? true : false;
    }

    protected void CoinDrop()
    {
        if (dropCoinValue <= 0 || isKillDirectly == false)
        {
            return;
        }

        GameObject coin = ObjectManager.Instance.itemPool.ObjectDequeue("CoinItem", transform.position);
        coin.GetComponentInChildren<CoinItem>().CoinValue = (int)Random.Range(dropCoinValue * 0.7f, dropCoinValue * 1.3f);
    }

    #endregion

    #region PUN_CALLBACK

    [PunRPC]
    public override void TakeDamage(int damage, bool critical)
    {
        base.TakeDamage(damage, critical);

        if (curHp <= 0)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(DestroyObject), RpcTarget.All);
            }
        }
        else
        {
            SoundManager.Instance.PlaySFX("MonsterHit");
            StartCoroutine(nameof(OnFlashEffect));
        }
    }

    [PunRPC]
    public override void DestroyObject()
    {
        SoundManager.Instance.PlaySFX("MonsterDeath");

        GameObject deathEf = ObjectManager.Instance.effectPool.ObjectDequeue("MonsterDeathEffect");
        deathEf.transform.position = transform.position;

        if(isKillDirectly)
        {
            ScoreManager.Instance.ScoreAdd(50);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            CoinDrop();
            WaveManager.Instance.MonsterDeath();
            ObjectManager.Instance.monsterPool.ObjectEnqueue(poolName, this.gameObject);
        }
    }

    [PunRPC]
    protected void GetPlayerRPC(int playerNum)
    {
        player = PlayerManager.Instance.GetPlayer(playerNum);
    }

    [PunRPC]
    public void SetKillDirectly(bool isCheck)
    {
        isKillDirectly = isCheck;
	}

    protected void SetTarget(Transform target)
    {
        aiDest.target = target;
    }
    
    protected void GetPlayerDistance(float senseDistance)
    {
        int minNum = -1;
        float minDistance = MAX_NUM;
        
        for (int i = 0; i < players.Length; i++)
        {
            int curNum = players[i].playerNum;
            if (players[i].state == PlayerState.Death)
            {
                continue;
            }

            float curDis = Vector2.Distance(players[i].transform.position, transform.position);
            if (curDis <= senseDistance)
            {
                if(minDistance > curDis)
                {
                    minNum = curNum;
                    minDistance = curDis;
                }
            }
        }
        if (minNum != -1)
        {
            photonView.RPC(nameof(GetPlayerRPC), RpcTarget.All, minNum);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isFlip);
        }
        else
        {
            isFlip = (bool)stream.ReceiveNext();
        }
    }

    #endregion

    #region COROUTINE

    protected IEnumerator OnFlashEffect()
    {
        var t_material = material;

        spriteRender.material = whiteFlashMaterial;
        yield return new WaitForSeconds(0.1f);
        spriteRender.material = t_material;
    }

    protected IEnumerator ChargingStartCo(float chargingTime, Action act)
    {
        float curTime = 0;

        attChBar.SetActive(true);
        while (curTime <= chargingTime && isAttack)
        {
            attChBarImg.fillAmount = curTime / chargingTime;
            curTime += 0.02f;

            if(player.state != PlayerState.Live)
            {
                isAttack = false;
			}
            yield return new WaitForSeconds(0.02f);
        }

        attChBar.SetActive(false);
        act();
    }

    protected IEnumerator FindPlayerNear(float distance)
    {
        do
        {
            GetPlayerDistance(distance);
            yield return new WaitForSeconds(0.3f);
        } while (player == null);
    }

    protected IEnumerator PlayerStateSearch(PlayerState state)
    {
        do
        {
            PlayerSearch();
            yield return new WaitForSeconds(0.3f);
        } while (player.state != state);
    }

    #endregion
}
