using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeMachine : DefenceMachine
{
    private Animator animator;

    public GameObject spikesColl;

    private float readyDelay;
    private bool onDamage = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(SpikesRepeat());
    }
    IEnumerator SpikesRepeat()
    {
        while(true)
        {
            onDamage = false;
            spikesColl.SetActive(false);
            yield return new WaitForSeconds(readyDelay);
            Fire();
            yield return new WaitForSeconds(operateTime);
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void Fire()
    {
        onDamage = true;
        Collider2D[] t_moveCheck = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.4f, 0.4f), 0f);

        foreach (Collider2D i in t_moveCheck)
        {
            if(i.CompareTag("Player"))
            {
                //Player.Instance.GetDamage(25);
            }
        }
        spikesColl.SetActive(true);
        animator.SetTrigger("Operate");
    }
}