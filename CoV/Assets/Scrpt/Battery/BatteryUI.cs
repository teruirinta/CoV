using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI�Q��")]
    public Image gaugeImage; // �~�`�Q�[�W
    public Image iconImage;  // �A�C�R���摜

    [Header("����UI���S�����鎋�E�^�C�v")]
    public VisionType targetVision; // NightScope / Inverted / Thermal

    private VisionManager visionManager;

    void Start()
    {
        visionManager = VisionManager.Instance;
    }

    void Update()
    {
        if (visionManager == null) return;

        // ����UI���S�����鎋�E�f�[�^���擾
        var vision = visionManager.GetVisionData(targetVision);
        if (vision == null) return;

        // �o�b�e���[�c�ʁi0�`1�ɐ��K���j
        float fillAmount = vision.currentBattery / vision.maxBattery;
        gaugeImage.fillAmount = Mathf.Clamp01(fillAmount);

        // �Q�[�W�̐F�ݒ�i�C�Ӂj
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
