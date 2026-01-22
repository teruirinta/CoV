using UnityEngine;

public class aEnemy : MonoBehaviour
{
    [Header("視界表示")]
    public GameObject parentPart;
    public GameObject childPart;

    [Header("プレイヤー関連")]
    public Transform player;
    public float detectionRange;

    [Header("移動ルート")]
    public Transform[] waypoints;
    public float moveSpeed;
    public float chaseSpeed;
    public float waypointThreshold = 0.5f;
    private int currentWaypointIndex = 0;

    [Header("足音設定")]
    public AudioSource footstepAudio;
    public float footstepTriggerRange;
    public float normalFootstepPitch;
    public float chaseFootstepPitch;

    private bool isDead = false;

    void Update()
    {
        if (isDead || VisionManager.Instance == null || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isPlayerNearby = distanceToPlayer <= detectionRange && CanSeePlayer();
        HandleFootstepAudio(distanceToPlayer, isPlayerNearby);

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

    void HandleFootstepAudio(float distanceToPlayer, bool isChasing)
    {
        if (footstepAudio == null) return;

        bool isMoving = (player.position - transform.position).magnitude > 0.01f;

        if (distanceToPlayer <= footstepTriggerRange && isMoving)
        {
            // 音量調整は削除済み
            footstepAudio.pitch = isChasing ? chaseFootstepPitch : normalFootstepPitch;

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
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            return hit.transform == player;
        }

        return false;
    }

    public void Die()
    {
        isDead = true;

        if (footstepAudio != null && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }

        // 必要ならここでアニメーションやエフェクトも追加できるよ！
    }
}
