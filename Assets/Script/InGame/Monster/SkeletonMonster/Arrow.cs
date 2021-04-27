using UnityEngine;
using Photon.Pun;

public class Arrow : Projectile
{
    private PlayerCollCheck playerColl;

    private void Start()
    {
        playerColl = new PlayerCollCheck(FindObjectsOfType<Player>());
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        
        GameObject effect = ObjectManager.Instance.effectPool.ObjectDequeue(hitEffectName);
        effect.transform.rotation = Quaternion.Euler(0, 0, angle);
        effect.transform.position = transform.position;
        
  
            if (collision.CompareTag("Player"))
            {
                if (collision.TryGetComponent<Player>(out Player player))
                {
                    collision.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.All, damage);
                    ObjectDestroy();
                } 
            
            if (collision.CompareTag("Wall"))
            {
                ObjectDestroy();
            }
        }
    }
}
