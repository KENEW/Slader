using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DefenceMachine : MonoBehaviourPunCallbacks
{
    protected delegate void operateFunc();
    protected operateFunc operatefunc;
    protected operateFunc readyfunc;
	
    [SerializeField] protected float periodTime;
    [SerializeField] protected float operateTime;
	
    public void StartOperate(float operateTime)
    {
        this.operateTime = operateTime;
        StartCoroutine(OperateTrapCo());
    }
    IEnumerator OperateTrapCo()
    {
        float curTime = 0.0f;

        while (curTime <= operateTime)
        {
            operatefunc();
            curTime += periodTime;
            yield return new WaitForSeconds(periodTime);
        }
        readyfunc();
    }
}
