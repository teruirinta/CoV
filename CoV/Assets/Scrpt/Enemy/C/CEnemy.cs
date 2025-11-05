using UnityEngine;

public class CEnemy : MonoBehaviour
{
    [Header("攻撃判定の距離（プレイヤーとの距離）")]
    public float detectionRange = 3f;   // プレイヤーに反応する距離

    [Header("襲い掛かる時の移動速度")]
    public float attackSpeed = 5f;      // 攻撃時の突進速度

    [Header("敵のRenderer（透明 → 可視化制御用）")]
    public Renderer enemyRenderer;      // 見える/見えないを切り替えるためのRenderer

    private Transform player;           // プレイヤーのTransform参照
    private bool isDead = false;        // 敵が死亡しているかの判定
    private bool isAttacking = false;   // 現在攻撃状態かどうか

    void Start()
    {
        // シーン内にいるPlayerオブジェクトをタグで取得
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 初期状態では敵は透明（存在を悟られない演出）
        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = false;
        }
    }

    void Update()
    {
        // 死亡していたら何もしない
        if (isDead) return;

        // プレイヤーとの距離を測る
        float distance = Vector3.Distance(transform.position, player.position);

        // プレイヤーが近距離に入ったら攻撃を開始
        if (!isAttacking && distance <= detectionRange)
        {
            StartAttack();
        }

        // 攻撃中はプレイヤーに向かってまっすぐ突進
        if (isAttacking)
        {
            transform.position = Vector3.MoveTowards
            (
               transform.position,
                player.position,
                attackSpeed * Time.deltaTime
            );
        }
    }

    /// プレイヤーに襲い掛かる（攻撃状態へ移行）

    void StartAttack()
    {
        isAttacking = true;

        // 姿が現れる（プレイヤーが気づいた瞬間の恐怖演出）
        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = true;
        }
    }

    /// 何かがこの敵のトリガーに触れた時に呼ばれる

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;   // 死亡後は無視

        // プレイヤーに触れたら即死
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Dead");
            // ここに実際のプレイヤー死亡処理を呼び出す
            // 例：
            // other.GetComponent<PlayerHealth>().Die();
        }

        // 塩に当たった場合は死亡
        if (other.CompareTag("Salt"))
        {
            Die();
        }
    }

    /// 敵の死亡処理
    void Die()
    {
        isDead = true;
        isAttacking = false;

        // 敵を見えなくする（倒した感を出す）
        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = false;
        }

        // 碰撞を無効化（もう当たり判定を持たない）
        GetComponent<Collider>().enabled = false;

        // 演出のため少し残してから消す（直消しでもOK）
        Destroy(gameObject, 0.5f);
    }
}