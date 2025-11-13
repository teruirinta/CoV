using UnityEngine;

public class aEnemy : MonoBehaviour
{
    [Header("視界表示")]
    public GameObject parentPart; // 通常視界で表示
    public GameObject childPart;  // ナイトスコープで表示

    [Header("プレイヤー関連")]
    public Transform player;      // プレイヤーのTransform
    public float detectionRange;  // プレイヤーが近くにいると判定する距離

    [Header("移動ルート")]
    public Transform[] waypoints; // 敵が移動するルート（ウェイポイント）
    public float moveSpeed;       // 移動速度
    public float chaseSpeed;      // プレイヤーを追いかけるときの速度
    public float waypointThreshold = 0.5f; // 次のウェイポイントに切り替える距離
    private int currentWaypointIndex = 0;

    [Header("足音設定")]
    public AudioSource footstepAudio;       // 足音用AudioSource
    public float footstepTriggerRange;      // 足音を鳴らす最大距離
    public float maxFootstepVolume;         // 足音の最大音量
    public float normalFootstepPitch;       // 通常時のピッチ
    public float chaseFootstepPitch;        // 追跡時のピッチ

    void Update()
    {
        if (VisionManager.Instance == null || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isPlayerNearby = distanceToPlayer <= detectionRange && CanSeePlayer();
        HandleFootstepAudio(distanceToPlayer);

        if (isPlayerNearby)
        {
            ChasePlayer();
        }
        else
        {
            MoveAlongRoute();
        }

        switch (VisionManager.Instance.CurrentVision)
        {
            case VisionType.Normal:
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

    void MoveAlongRoute()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distanceToWaypoint < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
        }
    }

    void HandleFootstepAudio(float distanceToPlayer)
    {
        if (footstepAudio == null) return;

        if (distanceToPlayer <= footstepTriggerRange)
        {
            float volumeScale = 1f - (distanceToPlayer / footstepTriggerRange);
            footstepAudio.volume = Mathf.Clamp(volumeScale * maxFootstepVolume, 0f, maxFootstepVolume);
            footstepAudio.pitch = distanceToPlayer <= detectionRange ? chaseFootstepPitch : normalFootstepPitch;

            if (!footstepAudio.isPlaying)
            {
                footstepAudio.loop = true;
                footstepAudio.Play();
            }
        }
        else
        {
            if (footstepAudio.isPlaying)
            {
                footstepAudio.Stop();
            }
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
    bool CanSeePlayer()
    {
        //float viewAngle = 0f;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 視野角チェック
        //if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
            {
                return hit.transform == player;
            }
        }

        return false;
    }

}
