using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DefenceArrow : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    { 
        base.OnTriggerEnter2D(collision);

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<Monster>(out Monster mosnter))
            {
                mosnter.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage, false);

                GameObject effect = ObjectManager.Instance.effectPool.ObjectDequeue(hitEffectName);
                effect.transform.position = collision.transform.position;

                ObjectDestroy();
            }
        }
    }
}
