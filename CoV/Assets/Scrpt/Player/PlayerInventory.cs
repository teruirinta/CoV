using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    // 所持品が変わったときに呼ばれるイベント（UI更新などに使える）
    public event Action OnInventoryChanged;

    // 鍵を持っているかどうかのフラグ
    private bool hasKey = false;

    // 他のスクリプトから読み取るためのプロパティ（読み取り専用）
    public bool HasKey => hasKey;

    // 鍵を手に入れた時に呼ぶ関数
    public void AddKey()
    {
        if (!hasKey)
        {
            hasKey = true;
            OnInventoryChanged?.Invoke(); // イベント通知
            Debug.Log("[Inventory] 鍵を取得しました。");
        }
    }

    // 鍵を使った時に呼ぶ関数
    public void UseKey()
    {
        if (hasKey)
        {
            hasKey = false;
            OnInventoryChanged?.Invoke(); // イベント通知
            Debug.Log("[Inventory] 鍵を使用しました。");
        }
    }
}