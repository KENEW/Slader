using UnityEngine;
using Photon.Pun;

public class BatBomb : DangerArray
{
    public GameObject destroyObj;
    private int damage;
	
    public void OnBomb()
    {
        SoundManager.Instance.PlaySFX("Bomb");

        GetDamage();

        GameObject bombEf = ObjectManager.Instance.effectPool.ObjectDequeue("MonsterDeathEffect");
        bombEf.transform.position = transform.position;

        if (PhotonNetwork.IsMasterClient)
        {
            transform.parent.GetComponent<PhotonView>().RPC("SetKillDirectly", RpcTarget.All, false);
            transform.parent.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
        } 
    }
    private void GetDamage()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
		}

        Collider2D[] hitBox = Physics2D.OverlapBoxAll(transform.position, new Vector2(2f, 2f), 0f);

        foreach (Collider2D i in hitBox)
        {
            if (i.transform.CompareTag("Player"))
            {
                if(i.TryGetComponent<Player>(out Player player))
                {
                    player.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.All, damage);
                }
            }
        }
    }
    public void SetInit(float time, int damage)
    {
        readyTime = time;
        this.damage = damage;
		
        OnOperate();
    }
}