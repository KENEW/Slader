using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FireBall : Projectile
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        destroyDelay = 0.0f;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage, critical);
        }
        if (collision.CompareTag("Interaction"))
        {
            collision.GetComponent<PhotonView>().RPC("OnInteraction", RpcTarget.All);
        }
    }
}
