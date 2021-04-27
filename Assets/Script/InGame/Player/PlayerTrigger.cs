using Photon.Pun;
using UnityEngine;

/// <summary>
/// 플레이어 트리거 충돌 영역을 관리해주는 클래스
/// </summary>
public class PlayerTrigger : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (collision.CompareTag("Coin"))
        {
            int coin = collision.GetComponent<CoinItem>().CoinDrop();
            CoinManager.Instance.AddCoin(coin);
        }
    }
}
