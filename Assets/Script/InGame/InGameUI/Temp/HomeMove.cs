using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class HomeMove : MonoSingleton<HomeMove>
{
	public GameObject character;

	public Transform setPos;
	public float SET_TILE_GAP = 0.6f;

	public struct Info
	{
		public int y;
		public int x;
		public int count;

		public Info(int y, int x, int count)
		{
			this.y = y;
			this.x = x;
			this.count = count; 
		}
	}
	public struct Pos
	{
		public int y;
		public int x;
		public Pos(int y, int x)
		{
			this.y = y;
			this.x = x;
		}
	}
	private int[,] HOME_MAP = new int[,]
	{
		{0, 0, 0, 0, 0, 0, -1},
		{0, 0, 0, -1, -1, -1, -1},
		{0, 0, 0, 0, 0, 0, 0},
		{-1, 0, 0, -1, -1, 0, -1},
		{-1, 0, 0, -1, -1, 0, -1},
		{-1, 0, 0, 0, 0, 0, -1}
	};
	private int[,] dirMove = new int[,]
	{
		{1, 0},
		{-1, 0},
		{0, 1},
		{0,-1}
	};

	private int startY = 0;
	private int startX = 0;

	//BFS
	public List<Pos> CharMove(int x, int y)
	{
		int[,] homeMap = (int[,])HOME_MAP.Clone();

		Queue<Info> q = new Queue<Info>();
		List<Pos> pos = new List<Pos>();
		Pos[,] prevs = new Pos[100, 100];

		Info startPos = new Info(startY, startX, 1);
		q.Enqueue(startPos);
	
		while (q.Count > 0)
		{
			Info cur = q.Dequeue();
		
			int curY = cur.y;
			int curX = cur.x;
			int cnt = cur.count;
		
			if (curY == y && curX == x)
			{
				int setY = prevs[curY, curX].y;
				int setX = prevs[curY, curX].x;
		
				pos.Add(new Pos(curY, curX));
				pos.Add(new Pos(setY, setX));
		
				while (true)
				{
					int t_y = prevs[setY, setX].y;
					int t_x = prevs[setY, setX].x;
		
					if (t_y == startY && t_x == startX)
					{
						pos.Add(new Pos(t_y, t_x));
						pos.Reverse();
						return pos;
					}

					setY = t_y;
					setX = t_x;
					pos.Add(new Pos(setY, setX));
				}
			}
			for (int i = 0; i < 4; i++)
			{
				int nextY = curY + dirMove[i, 0];
				int nextX = curX + dirMove[i, 1];
		
				if (nextY >= 0 && nextY < homeMap.GetLength(0) && nextX >= 0 && nextX < homeMap.GetLength(1))
				{
					if (homeMap[nextY, nextX] == 0)
					{
						homeMap[nextY, nextX] = homeMap[curY, curX] + 1;
						prevs[nextY, nextX] = new Pos(curY, curX);
		
						Info t_curPos;
						t_curPos.y = nextY;
						t_curPos.x = nextX;
						t_curPos.count = cnt + 1;
		
						q.Enqueue(t_curPos);
					}
				}
			}
		}
	
		return pos;
	}

	private bool arrivalCheck = true;

	private Coroutine moveStartCo;
	private Coroutine autoMoveCo;

	
	private IEnumerator PlayerAutoMove()
	{
		int t_index;
		Pos t_pos;
		float delay;

		while (true)
		{
			yield return new WaitForSeconds(0.2f);

			if (arrivalCheck)
			{
				delay = UnityEngine.Random.RandomRange(7.0f, 14.0f);

				yield return new WaitForSeconds(delay);

				while (true)
				{
					t_index = (int)UnityEngine.Random.RandomRange(0, possiblePos.Count - 1);
					t_pos = possiblePos[t_index];

					if (!posCheck(t_pos.x, t_pos.y))
					{
						moveStartCo = StartCoroutine(moveDelay(t_pos.x, t_pos.y, 0.75f));
						arrivalCheck = false;
						break;
					}
				}
			}
		}
	}
	List<Pos> possiblePos = new List<Pos>();

	private void Start()
	{
		PossibleMovePos();
		autoMoveCo = StartCoroutine(PlayerAutoMove());
	}
	private void PossibleMovePos()
	{
		for(int i = 0; i < HOME_MAP.GetLength(0); i++)
		{
			for(int j = 0; j < HOME_MAP.GetLength(1); j++)
			{
				if(HOME_MAP[i, j] != -1)
				{
					possiblePos.Add(new Pos(i, j));
				}
			}
		}
	}
	private Pos RandomPos()
	{
		int randPos = (int)UnityEngine.Random.RandomRange(0, possiblePos.Count - 1);
		return possiblePos[randPos];
	}
	public void GameStart()
	{
		if (autoMoveCo != null)  StopCoroutine(autoMoveCo);
		if (moveStartCo != null) StopCoroutine(moveStartCo);

		StartCoroutine(moveDelay(3, 5, 0.15f, true));
	}
	private bool posCheck(int x, int y)
	{
		return (startY == y && startX == x);
	}
	public GameObject fadePanel;
	IEnumerator moveDelay(int x, int y, float speed, bool gameStart = false)
	{
		if(!posCheck(x, y))
		{
			List<Pos> roadPos = CharMove(x, y).ToList();

			if (roadPos.Count != 0)
			{
				for (int i = 0; i < roadPos.Count; i++)
				{
					character.transform.DOMove(new Vector2(roadPos[i].x * SET_TILE_GAP, -roadPos[i].y * SET_TILE_GAP) - new Vector2(1.9f, -2.0f), speed).SetEase(Ease.Linear);

					startX = roadPos[i].x;
					startY = roadPos[i].y;

					yield return new WaitForSeconds(speed);
				}

				startX = roadPos[roadPos.Count - 1].x;
				startY = roadPos[roadPos.Count - 1].y;

				if(startX == 1 && (startY == 3 || startY == 4 || startY == 5))
				{
					CoinBubbleManager.Instance.CreateCoinBubble(startX, startY);
				}

				arrivalCheck = true;
			}

			if(gameStart == true)
			{
				character.transform.DOMoveY(-3.0f, 1f).SetEase(Ease.Linear).OnComplete(() => {
					fadePanel.SetActive(true);
					fadePanel.GetComponent<Image>().DOFade(1, 0.6f).OnComplete(() => { LoadScene.Instance.LoadStart("InGame"); });
				});
			}
		}
	}
}
