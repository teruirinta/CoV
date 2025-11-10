using UnityEngine;

public class CEnemy : MonoBehaviour
{
    [Header("攻撃判定の距離（プレイヤーとの距離）")]
    public float detectionRange = 3f;   // プレイヤーに反応する距離

    [Header("襲い掛かる時の移動速度")]
    public float attackSpeed = 5f;      // 攻撃時の突進速度

    [Header("敵のRenderer（透明 → 可視化制御用）")]
    public Renderer enemyRenderer;      // Thermal の時だけ表示する

    private Transform player;
    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 初期状態では透明
        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = false;
        }
            
    }

    void Update()
    {
        if (isDead) return;

        // --- Thermal視界なら敵が見える、その他は透明 ---
        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = (VisionManager.Instance.CurrentVision == VisionType.Thermal);
        }

        // プレイヤーとの距離を測る
        float distance = Vector3.Distance(transform.position, player.position);

        // プレイヤーが近くに入ったら襲い掛かる
        if (!isAttacking && distance <= detectionRange)
        {
            StartAttack();
        }

        // 攻撃中はプレイヤーに向かって突進
        if (isAttacking)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attackSpeed * Time.deltaTime
            );
        }

    }

    // --- Thermal表示管理 ---
    void HandleVisibility()
    {
        // Thermal 以外の時は透明
        bool shouldVisible = (VisionManager.Instance.CurrentVision == VisionType.Thermal);

        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = shouldVisible;
        }
           
    }

    // --- 一定距離に入ったら襲い掛かる ---
    void CheckPlayerDistance()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (!isAttacking && dist <= detectionRange)
        {
            StartAttack();
        }

        // 襲い中はプレイヤーに向かって突進
        if (isAttacking)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attackSpeed * Time.deltaTime
            );
        }
    }

    void StartAttack()
    {
        isAttacking = true;

        // Thermal に関係なく「襲い掛かる瞬間だけ姿が見える」演出したいならここで true にしても OK
        bool shouldVisible = (VisionManager.Instance.CurrentVision == VisionType.Thermal);
        {
            enemyRenderer.enabled = true;
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Dead");
            // プレイヤー死亡処理をここに
        }

        if (other.CompareTag("Salt"))
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        isAttacking = false;

        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = false;
        }

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 0.5f);
    }
}