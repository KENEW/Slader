using Photon.Pun;
using UnityEngine;

public class PlayerArrow : Projectile
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (collision.CompareTag("Enemy"))
        {
            GameObject effect = ObjectManager.Instance.effectPool.ObjectDequeue(hitEffectName);
        
            effect.transform.rotation = Quaternion.Euler(0, 0, angle);
            effect.transform.position = transform.position;

            collision.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage, critical);
            
            if (--penetrateNum <= 0)
            {
                ObjectDestroy();
            }
        }
        if (collision.CompareTag("Interaction"))
        {
            collision.GetComponent<PhotonView>().RPC("OnInteraction", RpcTarget.All);
        }
    }
}
