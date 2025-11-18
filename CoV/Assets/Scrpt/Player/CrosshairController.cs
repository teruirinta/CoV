using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public GameObject crosshairUI;

    void Start()
    {
        if (crosshairUI != null)
        {
            crosshairUI.SetActive(false); // 最初は非表示
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (crosshairUI != null)
            {
                crosshairUI.SetActive(true); // 照準モードで表示
            }
        }
        else
        {
            if (crosshairUI != null)
            {
                crosshairUI.SetActive(false); // 通常時は非表示
            }
        }
    }
}
