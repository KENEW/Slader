using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceLever : Interaction
{
    public DefenceMachine[] machines;
    private Animator animator;
    
    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        onInteractionDele = OnOperate;
        offInteractionDele = OffOperate;
    }
    public void OnOperate()
    {
        animator.SetBool("Operate", true);
        SoundManager.Instance.PlaySFX("LeverOperate");
        
        foreach (var machine in machines)
        {
            machine.StartOperate(workTime);
        }   
    }
    public void OffOperate()
    {
        animator.SetBool("Operate", false);
        SoundManager.Instance.PlaySFX("LeverReady");
    }
}