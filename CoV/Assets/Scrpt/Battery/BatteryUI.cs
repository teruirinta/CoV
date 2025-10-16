using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI�Q��")]
    public Image gaugeImage; // �~�`�Q�[�W
    public Image iconImage;  // �A�C�R���摜

    [Header("�ݒ�")]
    public VisionType targetVision; // �Ή����鎋�E�^�C�v

    private VisionManager visionManager;

    void Start()
    {
        visionManager = VisionManager.Instance;
    }

    void Update()
    {
        if (visionManager == null) return;

        // �Ή����鎋�E�̃f�[�^�擾
        var vision = visionManager.GetVisionData(targetVision);
        if (vision == null) return;

        // ���݂̎c�ʂ��Q�[�W�ɔ��f
        float fillAmount = vision.currentBattery / vision.maxBattery;
        gaugeImage.fillAmount = Mathf.Clamp01(fillAmount);

        // ���E���ƂɐF�ݒ�
        switch (targetVision)
        {
            case VisionType.NightScope:
                gaugeImage.color = Color.green;
                break;
            case VisionType.Inverted:
                gaugeImage.color = Color.cyan;
                break;
            case VisionType.Thermal:
                gaugeImage.color = Color.red;
                break;
            default:
                gaugeImage.color = Color.gray;
                break;
        }
    }
}
