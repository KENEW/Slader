using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 몬스터들의 정해진 스폰지역에서 생성을 관리합니다.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    private readonly float SUMMON_DELAY_MAX = 18.0f;
    private readonly float SUMMON_DELAY_MIN = 8.0f;

    [SerializeField] private string prefabPath = "Prefab/Monster/";

	public void MonsterSpawn()
    {
        StartCoroutine(MonsterSpawnCo());
	}
    IEnumerator MonsterSpawnCo()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(SUMMON_DELAY_MIN, SUMMON_DELAY_MAX));

        while (WaveManager.Instance.CheckMonsterNum() && (GameManager.Instance.gameState == GameState.GameStart))
        {
            GameObject spawnEf = ObjectManager.Instance.effectPool.ObjectDequeue("MonsterSpawnEffect");
            spawnEf.transform.position = this.transform.localPosition;

            string monsterName = WaveManager.Instance.MonsterSpawn();
            GameObject monster = ObjectManager.Instance.monsterPool.ObjectDequeue(monsterName, transform.position);

            yield return new WaitForSeconds(UnityEngine.Random.Range(SUMMON_DELAY_MIN, SUMMON_DELAY_MAX));
        }
    }
}
