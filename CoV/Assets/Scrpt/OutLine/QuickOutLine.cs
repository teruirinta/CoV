using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Renderer))]
public class QuickOutline : MonoBehaviour
{
    [Header("Outline設定")]
    public Color outlineColor = Color.yellow;
    [Range(0f, 10f)] public float outlineWidth = 2f;

    GameObject outlineObj;
    Renderer outlineRenderer;

    void Awake()
    {
        CreateOutlineObject();
        SetVisible(false); // 初期は非表示
    }

    void CreateOutlineObject()
    {
        // 既に作成済みなら何もしない
        if (outlineObj != null) return;

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning($"QuickOutline: {name} に MeshFilter が無いか Mesh がありません。");
            return;
        }

        outlineObj = new GameObject(name + "_Outline");
        outlineObj.transform.SetParent(transform, false);
        outlineObj.transform.localPosition = Vector3.zero;
        outlineObj.transform.localRotation = Quaternion.identity;
        outlineObj.transform.localScale = Vector3.one;

        var ofMf = outlineObj.AddComponent<MeshFilter>();
        ofMf.sharedMesh = mf.sharedMesh;

        outlineRenderer = outlineObj.AddComponent<MeshRenderer>();

        // URP用シェーダー名（先に作ったもの）を使う
        Shader s = Shader.Find("Hidden/QuickOutline/URP_Outline");
        if (s == null)
        {
            // fallback：Unlitでも動くようにしておく（ただし見た目は違う）
            s = Shader.Find("Unlit/Color");
            Debug.LogWarning("QuickOutline: 'Hidden/QuickOutline/URP_Outline' シェーダーが見つかりませんでした。Unlit/Color を代用します。");
        }

        Material mat = new Material(s);
        mat.SetColor("_OutlineColor", outlineColor);
        // シェーダープロパティ名が異なる場合は適宜合わせる（_OutlineWidth 等）
        if (mat.HasProperty("_OutlineWidth")) mat.SetFloat("_OutlineWidth", outlineWidth);

        outlineRenderer.material = mat;

        // 描画順を少し上にして見えやすく（必要なら調整）
        outlineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        outlineRenderer.receiveShadows = false;
    }

    /// <summary>
    /// アウトラインの表示・非表示。trueで表示、falseで非表示。
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (outlineObj == null) return;
        outlineObj.SetActive(visible);
    }

    /// <summary>
    /// ランタイムで色を変更したい場合に使う
    /// </summary>
    public void SetColor(Color c)
    {
        if (outlineRenderer == null) return;
        if (outlineRenderer.material != null && outlineRenderer.material.HasProperty("_OutlineColor"))
            outlineRenderer.material.SetColor("_OutlineColor", c);
    }
}
