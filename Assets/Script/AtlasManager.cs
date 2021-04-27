using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

[System.Serializable]
public class Atlas
{
	public string name;
	public Image image;
}
public class AtlasManager : MonoBehaviour
{
	public SpriteAtlas sprAtlas;
	public Atlas[] atlas;

	private void Start()
	{
		for (int i = 0; i < atlas.Length; i++)
		{
			atlas[i].image.sprite = sprAtlas.GetSprite(atlas[i].name);
		}
	}
}
