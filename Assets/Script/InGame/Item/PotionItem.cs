using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : ExpendabilityItem
{
	private void UseItem()
	{
		SoundManager.Instance.PlaySFX("PotionSFX");
		DestroyObject();
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.transform.CompareTag("Player"))
		{
			UseItem();
		}
	}
}