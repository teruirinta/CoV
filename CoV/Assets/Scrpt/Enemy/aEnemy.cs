using UnityEngine;

public class aEnemy : MonoBehaviour
{

    public GameObject parentPart; // 通常視界で表示
    public GameObject childPart;  // ナイトスコープで表示
    public Transform player;      // プレイヤーのTransform
    public float detectionRange; // プレイヤーが近くにいると判定する距離

    void Update()
    {
        if (VisionManager.Instance == null || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isPlayerNearby = distanceToPlayer <= detectionRange;

        switch (VisionManager.Instance.CurrentVision)
        {
            case VisionType.Normal:
                // プレイヤーが近くにいたら子供も表示
                SetVisibility(parentVisible: true, childVisible: isPlayerNearby);
                break;

            case VisionType.NightScope:
                SetVisibility(parentVisible: false, childVisible: true);
                break;

            default:
                SetVisibility(parentVisible: false, childVisible: false);
                break;
        }
    }

    void SetVisibility(bool parentVisible, bool childVisible)
    {
        if (parentPart)
        {
            var renderer = parentPart.GetComponent<Renderer>();
            if (renderer) renderer.enabled = parentVisible;
            var collider = parentPart.GetComponent<Collider>();
            if (collider) collider.enabled = parentVisible;
        }

        if (childPart)
        {
            var renderer = childPart.GetComponent<Renderer>();
            if (renderer) renderer.enabled = childVisible;
            var collider = childPart.GetComponent<Collider>();
            if (collider) collider.enabled = childVisible;
        }
    }
}
