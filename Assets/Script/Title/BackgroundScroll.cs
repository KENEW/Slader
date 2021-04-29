using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{    
    public SpriteRenderer spr;
    public Transform[] sprites;

    [Range(0.0f, 10.0f)]
    public float scrollSpeed;

    public int startIndex;
    public int endIndex;

    private float weight;

    private void Start()
    {
        weight = Camera.main.orthographicSize * Screen.width / Screen.height;
    }
    private void Update()
    {
        Move();
        Scorlling();
    }
	private void Move()
	{
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.left * scrollSpeed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }
	private void Scorlling()
    {
        if(sprites[endIndex].position.x + spr.bounds.size.x < weight)
        {
            Vector3 backSprPos = sprites[startIndex].transform.position;
            sprites[endIndex].transform.position = new Vector3(backSprPos.x + spr.bounds.size.x - 0.1f, backSprPos.y, backSprPos.z);

            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
		}
	}
}