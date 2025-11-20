using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneController.BackToBeforeScene(); // ãLò^Ç≥ÇÍÇΩÉVÅ[ÉìÇ…ñﬂÇÈ
        }
    }
}
