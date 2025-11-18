using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public event Action OnInventoryChanged;

    // 鍵IDを管理するセット（重複なし）
    private HashSet<string> ownedKeys = new HashSet<string>();

    // 鍵を追加
    public void AddKey(string keyId)
    {
        if (!ownedKeys.Contains(keyId))
        {
            ownedKeys.Add(keyId);
            OnInventoryChanged?.Invoke();
            Debug.Log($"[Inventory] 鍵「{keyId}」を取得しました。");
        }
    }

    // 鍵を使用（削除）
    public void UseKey(string keyId)
    {
        if (ownedKeys.Contains(keyId))
        {
            ownedKeys.Remove(keyId);
            OnInventoryChanged?.Invoke();
            Debug.Log($"[Inventory] 鍵「{keyId}」を使用しました。");
        }
    }

    // 鍵を持っているか確認
    public bool HasKey(string keyId)
    {
        return ownedKeys.Contains(keyId);
    }
}
