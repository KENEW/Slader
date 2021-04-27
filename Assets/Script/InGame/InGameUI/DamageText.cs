using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private TextMeshPro textMesh;
	private Color color;

	private float moveSpeed = 2.0f;
	private float damage;
	private bool criticalCheck = false;
	private bool isOperate = false;

	private void Awake()
	{
		if (textMesh == null)
			textMesh = GetComponent<TextMeshPro>();
	}
	private void Update()
	{
		if(isOperate)
		{
			transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
			color.a = Mathf.Lerp(color.a, 0, moveSpeed * Time.deltaTime);
			textMesh.color = color;
		}
	}
	private void OnDisable()
	{
		color = Color.white;
	}
	public void GetDamage(float damage, bool criticalCheck)
	{
		this.damage = damage;
		this.criticalCheck = criticalCheck;

		TextUpdate();
		isOperate = true;
	}
	public void TextUpdate()
	{
		color.a = 1.0f;
		textMesh.text = (int)damage + "";

		if (criticalCheck)
		{
			color = Color.yellow;
		}
		else
		{
			color = Color.white;
		}

		textMesh.color = color;
		ObjectManager.Instance.effectPool.ObjectEnqueue("DamageText", gameObject, 1.5f);
	}
}
