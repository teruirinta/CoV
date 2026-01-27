using UnityEngine;
using UnityEngine.UI;

public class GoalImageSetter : MonoBehaviour
{
    [Header("ステージごとのゴール画像スプライト")]
    public Sprite[] goalSprites;

    [Header("差し替えるImage")]
    public Image targetImage;

    void Start()
    {
        int floor = GameProgress.lastClearedFloor;

        if (floor >= 1 && floor <= goalSprites.Length && targetImage != null)
        {
            targetImage.sprite = goalSprites[floor - 1];
        }
    }
}
