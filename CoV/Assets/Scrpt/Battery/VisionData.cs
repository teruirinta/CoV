using UnityEngine;

[CreateAssetMenu(menuName = "Vision/VisionData")]
public class VisionData : ScriptableObject
{
    [Header("��{�ݒ�")]
    public string visionName = "Normal";

    [Header("�o�b�e���[�ݒ�")]
    public float maxBattery = 100f;     // �ő�e��
    public float currentBattery = 100f; // ���ݗe��
    public float drainRate = 5f;        // 1�b������̌�����

    public bool IsDepleted => currentBattery <= 0f; // �o�b�e���[�؂ꔻ��
}
