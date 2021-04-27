using UnityEngine;

public class UISafeAreaManager : MonoBehaviour
{
    // UISafeAreaManager - 
    // ������ s10 �� ȭ�� ũ�Ⱑ �ұ����� ȭ�鿡�� UI ǥ�������� �������ش�.

    //RectTransform �� ���ڷ� �޾� ����ϸ� �ȴ�.
    public static void ApplySafeAreaPosition(RectTransform rt)
    {
        Rect safeArea = Screen.safeArea;

        // Safe Array ��Ʈ�� ���� ��ǥ���� ����ȭ �� ��Ŀ ��ǥ�� ��ȯ
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        // ���� anchor x ��ǥ ���
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