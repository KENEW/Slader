using System;
using System.Collections;
using UnityEngine;

public class CoroutineManager : MonoSingleton<CoroutineManager>
{
	public void CallWaitForOneFrame(Action act)
	{
		StartCoroutine(DoCallWaitForOneFrame(act));
	}
	public void CallWaitForSeconds(float seconds, Action act)
	{
		StartCoroutine(DoCallWaitForSeconds(seconds, act));
	}
	private IEnumerator DoCallWaitForOneFrame(Action act)
	{
		yield return 0; 
		act();
	}
	private IEnumerator DoCallWaitForSeconds(float seconds, Action act)
	{
		yield return new WaitForSeconds(seconds);
		act();
	}
}

