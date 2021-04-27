using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class CoinItem : ExpendabilityItem
{
    [SerializeField] private int coinValue = 50;
	
    public int CoinValue
    {
        get 
        { 
            return coinValue + (int)(coinValue * (MyData.Instance.charData.coinLevel * 0.01f)); 
        }
        set
        { 
            coinValue = value;
        }
    }
    public int CoinDrop()
    {
        SoundManager.Instance.PlaySFX("CoinDrop");
        
        photonView.RPC(nameof(DestroyObject), RpcTarget.All);
        return CoinValue;
    }
}