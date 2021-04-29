using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// act를 이용해서 콜백을 쉽게 관리해준다.
/// </summary>
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

