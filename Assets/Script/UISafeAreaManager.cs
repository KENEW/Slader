using UnityEngine;

public class UISafeAreaManager : MonoBehaviour
{
    // UISafeAreaManager - 
    // 갤럭시 s10 등 화면 크기가 불균형한 화면에서 UI 표현범위를 조절해준다.

    //RectTransform 을 인자로 받아 사용하면 된다.
    public static void ApplySafeAreaPosition(RectTransform rt)
    {
        Rect safeArea = Screen.safeArea;

        // Safe Array 렉트를 절대 좌표에서 정규화 된 앵커 좌표롤 변환
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        // 기존 anchor x 좌표 사용
        anchorMin.x = rt.anchorMin.x;
        anchorMax.x = rt.anchorMax.x;

        anchorMin.y /= Screen.height;
        anchorMax.y /= Screen.height;

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
    }

    public RectTransform[] canvas;

	private void Start()
	{
        foreach(var rect in canvas)
        {
            ApplySafeAreaPosition(rect);
        }
    }
}