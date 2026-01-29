using UnityEngine;

public class aEnemy : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("プレイヤー")]
    public Transform player;
    public float detectionRange = 6f;
    public float attackRange = 1.5f;

    [Header("移動")]
    public Transform[] waypoints;
    public float moveSpeed = 1.5f;
    public float chaseSpeed = 3.0f;
    public float waypointThreshold = 0.5f;
    private int currentWaypointIndex = 0;

    [Header("足音")]
    public AudioSource footstepAudio;
    public float footstepTriggerRange = 5f;
    public float normalFootstepPitch = 1f;
    public float chaseFootstepPitch = 1.3f;

    private bool isDead = false;
    private bool isChasing = false;

    Renderer[] renderers;

    // =====================
    // 初期化
    // =====================
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        // 最初は透明
        SetVisibility(false);
    }

    // =====================
    // 更新
    // =====================
    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // プレイヤー検知
        isChasing = distance <= detectionRange && CanSeePlayer();

        // ★ 暗視カメラ中は強制可視化
        if (VisionManager.Instance != null && VisionManager.Instance.IsNightVisionActive)
        {
            SetVisibility(true);
        }
        else
        {
            SetVisibility(isChasing);
        }

        // 即死攻撃
        if (isChasing && distance <= attackRange)
        {
            Attack();
            return;
        }

        // 行動
        if (isChasing)
            ChasePlayer();
        else
            MoveAlongRoute();

        UpdateAnimation();
        HandleFootstepAudio(distance);
    }

    // =====================
    // 移動
    // =====================
    void MoveAlongRoute()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < waypointThreshold)
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        Rotate(dir);
    }

    void ChasePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * chaseSpeed * Time.deltaTime;
        Rotate(dir);
    }

    void Rotate(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            5f * Time.deltaTime
        );
    }

    // =====================
    // 攻撃（即死）
    // =====================
    void Attack()
    {
        if (animator)
            animator.SetTrigger("Attack");

        if (footstepAudio && footstepAudio.isPlaying)
            footstepAudio.Stop();

        // プレイヤー即死処理
        // player.GetComponent<PlayerLife>().Die();
    }

    // =====================
    // アニメーション
    // =====================
    void UpdateAnimation()
    {
        if (!animator) return;

        animator.SetBool("IsWalking", !isChasing);
        animator.SetBool("IsChasing", isChasing);
    }

    // =====================
    // 透明制御（URP）
    // =====================
    void SetVisibility(bool visible)
    {
        float alpha = visible ? 1f : 0f;

        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.GetColor("_BaseColor");
                    c.a = alpha;
                    mat.SetColor("_BaseColor", c);
                }
            }
        }
    }

    // =====================
    // 足音
    // =====================
    void HandleFootstepAudio(float distance)
    {
        if (!footstepAudio) return;

        if (distance <= footstepTriggerRange)
        {
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
                footstepAudio.Stop();
        }
    }

    // =====================
    // 視線判定
    // =====================
    bool CanSeePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dist))
            return hit.transform == player;

        return false;
    }
}
