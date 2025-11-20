using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;   // 出したい敵
        public Transform spawnPoint;     // 出したい場所
    }

    public List<EnemySpawnData> spawnDataList = new List<EnemySpawnData>();
    private bool hasSpawned = false;

    public void SpawnEnemyOnce()
    {
        if (!hasSpawned)
        {
            foreach (var data in spawnDataList)
            {
                if (data.enemyPrefab != null && data.spawnPoint != null)
                {
                    Instantiate(data.enemyPrefab, data.spawnPoint.position, Quaternion.identity);
                }
            }

            hasSpawned = true;
            Debug.Log($"[EnemyManager] 敵を{spawnDataList.Count}体出現させました！");
        }
    }
}
