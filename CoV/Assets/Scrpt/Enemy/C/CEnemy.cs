using UnityEngine;
using UnityEngine.SceneManagement;

public class CEnemy : MonoBehaviour
{
    [Header("攻撃判定の距離（プレイヤーとの距離）")]
    public float detectionRange = 3f;

    [Header("襲い掛かる時の移動速度")]
    public float attackSpeed = 5f;

    [Header("元の位置に戻る速度")]
    public float returnSpeed = 2f;   // ▼追加：戻るときの速度

    [Header("敵のRenderer（透明 → 可視化制御用）")]
    public Renderer enemyRenderer;

    private Transform player;
    private bool isDead = false;
    private bool isAttacking = false;

    private bool playerIsVisible = false;

    // ▼追加：初期位置
    private Vector3 initialPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ▼追加：開始位置を保存
        initialPosition = transform.position;

        if (enemyRenderer != null)
            enemyRenderer.enabled = false;
    }

    void Update()
    {
        if (isDead) return;

        // ① Thermal視界でプレイヤーが見えるか？
        playerIsVisible = (VisionManager.Instance.CurrentVision == VisionType.Thermal);

        // ② プレイヤーが見えない → 追跡停止 → 初期位置へ戻る
        if (!playerIsVisible)
        {
            isAttacking = false;

            // 姿を消す
            if (enemyRenderer != null)
                enemyRenderer.enabled = false;

            // ▼追加：初期位置へ戻る
            ReturnToInitialPosition();
            return;
        }

        // ③ Thermal状態なら見える
        if (enemyRenderer != null)
            enemyRenderer.enabled = true;

        // ④ 距離判定して追跡開始
        float distance = Vector3.Distance(transform.position, player.position);
        if (!isAttacking && distance <= detectionRange)
        {
            StartAttack();
        }

        // ⑤ 追跡中はプレイヤーに向かって移動
        if (isAttacking)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attackSpeed * Time.deltaTime
            );
        }
    }

    // ▼追加：初期位置に戻る処理
    void ReturnToInitialPosition()
    {
        // 初期位置に近づく
        transform.position = Vector3.MoveTowards(
            transform.position,
            initialPosition,
            returnSpeed * Time.deltaTime
        );

        // 戻りきったら透明・待機状態
        float dist = Vector3.Distance(transform.position, initialPosition);
        if (dist < 0.1f)
        {
            if (enemyRenderer != null)
                enemyRenderer.enabled = false;

            isAttacking = false;
        }
    }

    void StartAttack()
    {
        isAttacking = true;

        if (enemyRenderer != null)
            enemyRenderer.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Dead");
            SceneManager.LoadScene("GameOver");
        }

        if (other.CompareTag("Salt"))
            Die();
    }

    void Die()
    {
        isDead = true;
        isAttacking = false;

        if (enemyRenderer != null)
            enemyRenderer.enabled = false;

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 0.5f);
    }
}
