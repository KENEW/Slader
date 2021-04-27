﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class DangerArray : MonoBehaviourPunCallbacks
{
    public SpriteRenderer spriteRender;

    public Color32 startColor;
 
    protected float readyTime = 2.0f;
    protected int blinkCycle = 6;

	protected void OnOperate()
    {
        startColor = spriteRender.color;
        StartCoroutine(Invincibility());
    }
    protected virtual void OnDisable()
    {
        StopCoroutine(Invincibility());
    }
    protected IEnumerator Invincibility()
    {
        float countTime = 0;
        while (countTime < readyTime + 0.3f)
        {
            if (countTime % 2 == 0)
            {
                spriteRender.color = new Color32(startColor.r, startColor.g, startColor.b, 30);
            }
            else
            {
                spriteRender.color = new Color32(startColor.r, startColor.g, startColor.b, 80);
            }
        
            yield return new WaitForSeconds(readyTime / (float)blinkCycle);
            countTime += readyTime / (float)blinkCycle;
        }

        spriteRender.color = startColor;
    }
}