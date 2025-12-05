using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowTarget : MonoBehaviour
{
    [Header("MemoryVision 時の発光マテリアル")]
    public Material glowMaterial;

    [Header("通常マテリアル（設定しなくても自動取得）")]
    public Material normalMaterial;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // 通常マテリアル未設定なら今のマテリアルを保存
        if (normalMaterial == null)
            normalMaterial = sr.material;
    }

    /// <summary>
    /// MemoryVision の ON/OFF で VisionManager から呼ばれる
    /// </summary>
    public void SetGlow(bool enableGlow)
    {
        if (sr == null) return;

        if (enableGlow)
        {
            if (glowMaterial != null)
                sr.material = glowMaterial;
        }
        else
        {
            // 元に戻す
            if (normalMaterial != null)
                sr.material = normalMaterial;
        }
    }
}
