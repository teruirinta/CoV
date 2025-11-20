using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public event Action OnInventoryChanged;
    public EnemyManager enemyManager;

    private HashSet<string> ownedKeys = new HashSet<string>();

    [Serializable]
    public class KeyData
    {
        public string keyId; // 鍵のID
    }

    public List<KeyData> keyDatas = new List<KeyData>();

    public void AddKey(string keyId)
    {
        if (!ownedKeys.Contains(keyId))
        {
            ownedKeys.Add(keyId);
            Debug.Log($"[Inventory] 鍵「{keyId}」を取得しました。");
            OnInventoryChanged?.Invoke();

            if (enemyManager != null)
            {
                enemyManager.SpawnEnemyOnce();
            }
        }
    }

    public void UseKey(string keyId)
    {
        if (ownedKeys.Contains(keyId))
        {
            ownedKeys.Remove(keyId);
            Debug.Log($"[Inventory] 鍵「{keyId}」を使用しました。");
            OnInventoryChanged?.Invoke();
        }
    }

    public bool HasKey(string keyId)
    {
        return ownedKeys.Contains(keyId);
    }
}
