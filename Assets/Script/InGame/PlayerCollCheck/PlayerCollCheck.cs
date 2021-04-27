using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Update문과 같이 연속된 프레임에서 플레이어 충돌체크 여부를 알려주고 관리해주는 클래스.
/// </summary>
public class PlayerCollCheck
{
    public Dictionary<int, bool> playerColl;

    public PlayerCollCheck(Player[] players)
    {
        playerColl = new Dictionary<int, bool>();
        
        foreach (var ply in players)
        {
            playerColl.Add(ply.playerNum, false);
        }
    }
    public void Init()
    {
        for(int i = 0; i < playerColl.Count; i++)
        {
            playerColl[i] = false;
        }
    }
    public void OnColl(int playerNum)
    {
        playerColl[playerNum] = true;
    }
    public bool CheckPlayer(int playerNum)
    {
        return !playerColl[playerNum];
    }
}