using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImg : MonoBehaviour
{
    private List<GameObject> trailParts = new List<GameObject>();

    public void OnEffect()
    {
        InvokeRepeating("SpawnTrailPart", 0, 0.3f);
    }
    public void OffEffect()
    {
        CancelInvoke();
    }
    private void SpawnTrailPart()
    {
        GameObject trailPart = new GameObject();
        SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
        trailPartRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
        trailPart.transform.position = transform.position;
        trailPart.transform.localScale = transform.localScale;

        trailParts.Add(trailPart);

        StartCoroutine(FadeTrailPart(trailPartRenderer));
        StartCoroutine(DestroyTrailPart(trailPart, 0.7f));
    }

    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
    {
        Color color = trailPartRenderer.color;
        color.a -= 0.8f;
        trailPartRenderer.color = color;

        yield return new WaitForEndOfFrame();
    }

    IEnumerator DestroyTrailPart(GameObject trailPart, float delay)
    {
        yield return new WaitForSeconds(delay);

        trailParts.Remove(trailPart);
        Destroy(trailPart);
    }
}