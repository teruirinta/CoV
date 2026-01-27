
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 待ち伏せ型の敵
/// ・通常は透明
/// ・プレイヤーが近づく or MemoryVisionで見られると出現
/// ・視線が通って近距離に入ると攻撃
/// </summary>
public class CEnemy : MonoBehaviour
{
    [Header("攻撃判定の距離（プレイヤーとの距離）")]
    public float detectionRange = 3f;

    [Header("襲い掛かる時の移動速度")]
    public float attackSpeed = 5f;

    [Header("元の位置に戻る速度")]
    public float returnSpeed = 2f;

    [Header("敵のRenderer（透明 → 可視化制御用）")]
    public Renderer enemyRenderer;

    [Header("表示される距離（プレイヤーとの距離）")]
    public float appearRange = 6f;

    [Header("呼吸音")]
    public AudioSource breathingAudio;

    [Header("Animator（Run / Attack制御）")]
    public Animator animator;

    // プレイヤー参照
    private Transform player;

    // 死亡フラグ
    private bool isDead = false;

    // 攻撃中フラグ
    private bool isAttacking = false;

    // MemoryVisionで見えているか
    private bool playerIsVisible = false;

    // 初期位置（戻り用）
    private Vector3 initialPosition;

    void Start()
    {
        // プレイヤー取得
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 初期位置を保存
        initialPosition = transform.position;

        // 初期状態は透明
        if (enemyRenderer != null)
            enemyRenderer.enabled = false;

        // 呼吸音を常時再生
        if (breathingAudio != null)
        {
            breathingAudio.loop = true;
            breathingAudio.Play();
        }

        // Animator 初期化（待機状態）
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAttacking", false);
    }

    void Update()
    {
        // 死亡後は何もしない
        if (isDead) return;

        // プレイヤーとの距離
        float distance = Vector3.Distance(transform.position, player.position);

        // MemoryVisionで見られているか
        playerIsVisible = (VisionManager.Instance.CurrentVision == VisionType.MemoryVision);

        // 出現条件：近い or MemoryVision
        bool shouldAppear = playerIsVisible || distance <= appearRange;

        // ===== 視線チェック（Raycast）=====
        bool hasLineOfSight = false;
        RaycastHit hit;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hasLineOfSight = true;
            }
        }

        // 攻撃条件：距離 + 視線
        bool shouldAttack = distance <= detectionRange && hasLineOfSight;

        // ===== 出現条件を満たしていない =====
        if (!shouldAppear)
        {
            // 攻撃解除
            isAttacking = false;

            // 透明化
            if (enemyRenderer != null)
                enemyRenderer.enabled = false;

            // Animatorも待機状態に戻す
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsAttacking", false);

            // 初期位置に戻る
            ReturnToInitialPosition();
            return;
        }

        // ===== 出現状態 =====
        if (enemyRenderer != null)
            enemyRenderer.enabled = true;

        // 攻撃していない間は走りアニメーション
        animator.SetBool("IsRunning", !isAttacking);

        // 攻撃条件を満たしたら攻撃開始
        if (!isAttacking && shouldAttack)
        {
            StartAttack();
        }

        // ===== 攻撃中の移動 =====
        if (isAttacking)
        {
            animator.SetBool("IsAttacking", true);

            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attackSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// 初期位置へ戻る処理
    /// </summary>
    void ReturnToInitialPosition()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            initialPosition,
            returnSpeed * Time.deltaTime
        );

        // 元の位置に戻ったら完全待機
        float dist = Vector3.Distance(transform.position, initialPosition);
        if (dist < 0.1f)
        {
            if (enemyRenderer != null)
                enemyRenderer.enabled = false;

            isAttacking = false;

            animator.SetBool("IsRunning", false);
            animator.SetBool("IsAttacking", false);
        }
    }

    /// <summary>
    /// 攻撃開始処理
    /// </summary>
    void StartAttack()
    {
        isAttacking = true;

        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAttacking", true);

        if (enemyRenderer != null)
            enemyRenderer.enabled = true;
    }

    /// <summary>
    /// 接触判定
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        // プレイヤーに触れたらゲームオーバー
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("GameOver");
        }

        // 塩に当たったら死亡
        if (other.CompareTag("Salt"))
            Die();
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    void Die()
    {
        isDead = true;
        isAttacking = false;

        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAttacking", false);

        if (enemyRenderer != null)
            enemyRenderer.enabled = false;

        if (breathingAudio != null)
            Destroy(breathingAudio.gameObject);

        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 0.5f);
    }
}
