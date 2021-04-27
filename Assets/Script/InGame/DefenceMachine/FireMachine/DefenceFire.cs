using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DefenceFire : Projectile
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
                ObjectDestroy();
            }
        }
    }
}