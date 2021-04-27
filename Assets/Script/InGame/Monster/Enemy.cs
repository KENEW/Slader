using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Enemy : MonoBehaviourPunCallbacks
{
    protected Camera camera;
    protected GameObject hpBar;
    protected Image hpBarImg;
    protected SpriteRenderer spriteRender;
    protected Rigidbody2D rigid;

    protected Material material;
    protected Material normalMeterial;
    public Material whiteFlashMaterial;

    [SerializeField] protected string poolName;

    protected float maxHp;
    protected float curHp;
    private float damageTime = 0.0f;
    private bool damageCheck = false;

    protected bool isInit = false;
    protected bool isActive = false;

    #region PUN_CALLBACK
    [PunRPC]
    public virtual void TakeDamage(int damage, bool critical)
    {
        GameObject damageText = ObjectManager.Instance.effectPool.ObjectDequeue("DamageText");
        
        damageText.transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
        damageText.GetComponent<DamageText>().GetDamage(damage, critical);

        curHp -= damage;

        damageCheck = true;
        damageTime = 0f;

        hpBarImg.fillAmount = curHp / maxHp;
    }
    [PunRPC]
    public virtual void DestroyObject()
    {
        ObjectManager.Instance.uIPool.ObjectEnqueue("MonsterHpBar", hpBar);
        
        if (PhotonNetwork.IsMasterClient)
        {
            ObjectManager.Instance.monsterPool.ObjectEnqueue(poolName, this.gameObject);
        }
    }
    #endregion

	#region UNITY
    public override void OnEnable()
    {
        base.OnEnable();

        if (!isInit)
        {
            rigid = GetComponent<Rigidbody2D>();
            spriteRender = GetComponent<SpriteRenderer>();
            material = GetComponent<SpriteRenderer>().material;
            camera = Camera.main;
            normalMeterial = material;
            isInit = true;
        }
        else
        {
            spriteRender.material = normalMeterial;
            hpBar = ObjectManager.Instance.uIPool.ObjectDequeue("MonsterHpBar");
            hpBarImg = hpBar.transform.GetChild(0).GetComponent<Image>();

            hpBarImg.fillAmount = 1.0f;
            hpBar.SetActive(false);
            isActive = true;
        }

        curHp = maxHp;
    }
    protected virtual void Update()
    {
        if(!isInit || hpBar == null)
        {
            return;
		}

        hpBar.transform.position = camera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + 0.5f));
        HideHp();
    }
    protected void HideHp()
    {
        if (damageCheck)
        {
            damageTime += Time.deltaTime;

            if (damageTime >= 2.5f)
            {
                hpBar.SetActive(false);
                damageCheck = false;
            }
            else
            {
                hpBar.SetActive(true);
            }
        }
    }
    #endregion
}