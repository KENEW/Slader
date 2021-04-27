using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoSingleton<ObjectManager>
{
	public ServerPoolManager monsterPool;

	public PoolManager effectPool;
	public PoolManager projectilePool;
	public ServerPoolManager itemPool;
	public PoolManager uIPool;

	public GameObject doorTarget;
}
