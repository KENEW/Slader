using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
	Down,
	Left,
	Right,
	Up
}
public static class GameUtility
{
	public static void ShuffleArray<T>(T[] array)
	{
		int random1;
		int random2;

		T tmp;

		for (int index = 0; index < array.Length; ++index)
		{
			random1 = UnityEngine.Random.Range(0, array.Length);
			random2 = UnityEngine.Random.Range(0, array.Length);

			tmp = array[random1];
			array[random1] = array[random2];
			array[random2] = tmp;
		}
	}
	public static void ShuffleList<T>(List<T> list)
	{
		int random1;
		int random2;

		T tmp;

		for (int index = 0; index < list.Count; ++index)
		{
			random1 = UnityEngine.Random.Range(0, list.Count);
			random2 = UnityEngine.Random.Range(0, list.Count);

			tmp = list[random1];
			list[random1] = list[random2];
			list[random2] = tmp;
		}
	}
	public static Vector2 DirConv(Direction dir)
	{
		switch (dir)
		{
			case Direction.Right:
				return Vector2.right;
			case Direction.Left:
				return Vector2.left;
			case Direction.Down:
				return Vector2.down;
			case Direction.Up:
				return Vector2.up;
			default:
				return Vector2.one;
		}
	}
	public static float angleConv(Direction dir)
	{
		switch (dir)
		{
			case Direction.Right:
				return 90;
			case Direction.Left:
				return 270;
			case Direction.Down:
				return 180;
			case Direction.Up:
				return 0;
			default:
				return 0;
		}
	}
	public static float GetAngleBetween(Vector2 to, Vector2 from)
	{
		Vector2 dir = (to - from);

		return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
	}
	public static Vector2 RotatePointAroundTransform(Vector2 dir, float angles)
	{
		float degree = angles * Mathf.Deg2Rad;
		float x = Mathf.Cos(degree) * (dir.x) - Mathf.Sin(degree) * (dir.y);
		float y = Mathf.Sin(degree) * (dir.x) + Mathf.Cos(degree) * (dir.y);

		return new Vector2(x, y);
	}
}