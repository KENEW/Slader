using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	protected const int MAX_PENETRATE = 100;

	[Header("PoolName")]
	[SerializeField] protected string objectName;
	[SerializeField] protected string hitEffectName;
	[SerializeField] protected string destroyEffectName;
	
	protected float moveSpeed = 2.0f;
	protected float angle = 0f;
	protected float destroyDelay = 0.0f;
	protected int damage = 0;
	protected int penetrateNum = 1;
	protected bool critical = false;

	protected Vector2 curDir = Vector2.up;

	private Coroutine sustainDesCo;

	protected virtual void FixedUpdate()
	{
		transform.Translate(curDir * moveSpeed * Time.deltaTime);
	}
	protected virtual void OnTriggerEnter2D(Collider2D collision) { }
	public void GetInfo(int damage, bool critical, Vector2 dir, float moveSpeed = 3.5f, int penetrateNum = MAX_PENETRATE, float angle = 0, float destroyDelay = 0.0f, float destroySustain = 2.0f)
	{
		this.moveSpeed = moveSpeed;
		this.damage = damage;
		this.critical = critical;
		this.curDir = dir;
		this.angle = angle;
		this.penetrateNum = penetrateNum;
		this.destroyDelay = destroyDelay;

		transform.rotation = Quaternion.Euler(0, 0, angle);
		sustainDesCo = StartCoroutine(SustainDestroy(destroySustain));
	}
	protected void ObjectDestroy()
	{
		StopCoroutine(sustainDesCo);
		CoroutineManager.Instance.CallWaitForSeconds(destroyDelay, () => { 
			ObjectManager.Instance.projectilePool.ObjectEnqueue(objectName, this.gameObject); 
		});
	}
	IEnumerator SustainDestroy(float destroySustain)
	{
		yield return new WaitForSeconds(destroySustain);
		ObjectDestroy();
	}
}
