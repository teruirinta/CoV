using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("敵のプレハブ（今は空でもOK）")]
    public GameObject enemyPrefab;

    [Header("通常時のスポーン位置")]
    public Transform[] defaultSpawnPoints;

    [Header("鍵取得後のスポーン位置（敵を変えたい時用）")]
    public Transform[] keyPickedSpawnPoints;

    // スポーンした敵のリスト
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 敵プレハブがセットされていれば生成
        if (enemyPrefab != null && defaultSpawnPoints.Length > 0)
        {
            SpawnAt(defaultSpawnPoints);
        }
        else
        {
            Debug.Log("[EnemySpawner] 敵が設定されていないため、スキップします。");
        }
    }

    // 指定した位置に敵を生成
    private void SpawnAt(Transform[] points)
    {
        ClearEnemies();

        foreach (var p in points)
        {
            if (p == null || enemyPrefab == null) continue;
            var e = Instantiate(enemyPrefab, p.position, p.rotation);
            spawnedEnemies.Add(e);
        }
    }

    // 既存の敵を消す
    private void ClearEnemies()
    {
        foreach (var e in spawnedEnemies)
        {
            if (e != null) Destroy(e);
        }
        spawnedEnemies.Clear();
    }

    // 鍵を拾った時に呼ばれる（KeyPickup から）
    public void OnKeyPickedUp(string keyId)
    {
        if (enemyPrefab == null)
        {
            // 敵がいない場合はログだけ出す
            Debug.Log("[EnemySpawner] 鍵を拾ったが、敵は存在しません。");
            return;
        }

        Debug.Log("[EnemySpawner] 鍵取得により敵配置を変更します。");
        SpawnAt(keyPickedSpawnPoints);
    }
}