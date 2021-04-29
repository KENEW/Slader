using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public interface IInteraction
{
    void Operation();
}
/// <summary>
/// 상호작용이 있는 오브젝트를 처리해준다.
/// </summary>
public class Interaction : MonoBehaviourPunCallbacks
{
    private const float PROGRESS_TIME = 0.05f;

    protected delegate void InteractionDele();

    protected InteractionDele onInteractionDele;
    protected InteractionDele offInteractionDele;
    
    [SerializeField] protected GameObject workingBar;
    
    protected Image workingBarImg;
    protected Image workingBarOutImg;

    [SerializeField] protected float coolTime;
    [SerializeField] protected float workTime;
    
    protected bool isWorking = false;
    protected bool isCharging = false;
    
    protected virtual void Start()
    {
        workingBarImg = workingBar.transform.GetChild(0).GetComponent<Image>();
        workingBarOutImg = workingBar.GetComponent<Image>();
        
        workingBar.SetActive(false);
    }
    [PunRPC]
    public virtual void OnInteraction()
    {
        if (!isWorking && !isCharging)
        {
            StartCoroutine(WorkingCo());
        }
    }
    [PunRPC]
    public virtual void OffInteraction()
    {
        offInteractionDele();
        workingBar.SetActive(false);
        isCharging = false;
    }
    protected IEnumerator WorkingCo()
    {
        isWorking = true;

        Color color = workingBarImg.color;
        color.a = 1.0f;
        workingBarImg.color = color;

        Color colorOut = workingBarOutImg.color;
        color.a = 1.0f;
        workingBarOutImg.color = colorOut;

        onInteractionDele();
        workingBar.SetActive(true);
        float curTime = workTime;
        while (curTime >= 0.0f)
        {
            workingBarImg.fillAmount = curTime / workTime;
            curTime -= PROGRESS_TIME;
            yield return new WaitForSecondsRealtime(PROGRESS_TIME);
        }
        
        isWorking = false;
        StartCoroutine(ChargingCo());
    }
    protected IEnumerator ChargingCo()
    {
        isCharging = true;

        Color color = workingBarImg.color;
        color.a = 0.5f;
        workingBarImg.color = color;

        Color colorOut = workingBarOutImg.color;
        color.a = 0.5f;
        workingBarOutImg.color = colorOut;

        float curTime = 0.0f;
        while (curTime < coolTime)
        {
            workingBarImg.fillAmount = curTime / coolTime;
            curTime += PROGRESS_TIME;

  
            yield return new WaitForSecondsRealtime(PROGRESS_TIME);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(OffInteraction), RpcTarget.All);
        }
    }
}