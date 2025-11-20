using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public event Action OnInventoryChanged;
    public EnemyManager enemyManager;
    private HashSet<string> ownedKeys = new HashSet<string>();

    // RawImage用の鍵UIデータ
    [Serializable]
    public class KeyUIData
    {
        public string keyId;               // 鍵のID
    }

    public List<KeyUIData> keyUIDatas = new List<KeyUIData>();

    public void AddKey(string keyId)
    {
        if (!ownedKeys.Contains(keyId))
        {
            ownedKeys.Add(keyId);
            Debug.Log($"[Inventory] 鍵「{keyId}」を取得しました。");
            OnInventoryChanged?.Invoke();

            // 敵を出現させる
            if (enemyManager != null)
            {
                enemyManager.SpawnEnemyOnce(); // ← EnemyManagerにこの関数を作ってね！
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
