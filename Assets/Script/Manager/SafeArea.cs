using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 갤럭시S10, 20, 아이폰X11등 화면 크기가 불균형한 화면에서 UI 표현범위를 조절해준다.
/// 오브젝트에 이 스크립트를 붙여넣으면 된다.
/// </summary>
public class SafeArea : MonoBehaviour
{
	private RectTransform rectTransform;
	private Rect safeArea;
	private Vector2 minAnchor;
	private Vector2 maxAnchor;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		safeArea = Screen.safeArea;
		minAnchor = safeArea.position;
		maxAnchor = minAnchor + safeArea.size;

		minAnchor.y /= Screen.height;
		maxAnchor.y /= Screen.height;
		minAnchor.x /= Screen.width;
		maxAnchor.x /= Screen.width;

		rectTransform.anchorMin = minAnchor;
		rectTransform.anchorMax = maxAnchor;
	}

}
