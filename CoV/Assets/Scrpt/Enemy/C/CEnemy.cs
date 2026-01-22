using UnityEngine;
using UnityEngine.SceneManagement;

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

    private Transform player;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool playerIsVisible = false;
    private Vector3 initialPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        initialPosition = transform.position;

        if (enemyRenderer != null)
            enemyRenderer.enabled = false;

        if (breathingAudio != null)
        {
            breathingAudio.loop = true;
            breathingAudio.Play();
        }
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerIsVisible = (VisionManager.Instance.CurrentVision == VisionType.MemoryVision);

        bool shouldAppear = playerIsVisible || distance <= appearRange;

        // Raycastで視線チェック
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

        bool shouldAttack = distance <= detectionRange && hasLineOfSight;

        if (!shouldAppear)
        {
            isAttacking = false;

            if (enemyRenderer != null)
                enemyRenderer.enabled = false;

            ReturnToInitialPosition();
            return;
        }

        if (enemyRenderer != null)
            enemyRenderer.enabled = true;

        if (!isAttacking && shouldAttack)
        {
            StartAttack();
        }

        if (isAttacking)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attackSpeed * Time.deltaTime
            );
        }
    }

    void ReturnToInitialPosition()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            initialPosition,
            returnSpeed * Time.deltaTime
        );

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

        if (breathingAudio != null)
            Destroy(breathingAudio.gameObject);

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 0.5f);
    }
}
