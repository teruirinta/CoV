using System.Collections.Generic;
using UnityEngine;

public class BatterySpawner : MonoBehaviour
{
    public GameObject batteryPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 30f;
    public int maxBatteryCount = 2;

    private List<GameObject> spawnedBatteries = new List<GameObject>();

    void Start()
    {
        InvokeRepeating("TrySpawnBattery", 0f, spawnInterval);
    }

    void TrySpawnBattery()
    {
        // 現在のバッテリー数を確認（nullになったものは除外）
        spawnedBatteries.RemoveAll(item => item == null);

        if (spawnedBatteries.Count >= maxBatteryCount)
            return;

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnLocation = spawnPoints[index];

        GameObject newBattery = Instantiate(batteryPrefab, spawnLocation.position, Quaternion.identity);
        spawnedBatteries.Add(newBattery);
    }
}
