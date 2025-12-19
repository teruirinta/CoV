using UnityEngine;

public class WavyWall : MonoBehaviour
{
    public float amplitude = 0.05f; // —h‚ê‚Ì‘å‚«‚³
    public float frequency = 1f;    // —h‚ê‚Ì‘¬‚³

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = initialPosition + new Vector3(0f, offset, 0f);
    }
}
