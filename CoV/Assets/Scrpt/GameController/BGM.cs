using UnityEngine;
using System.Collections;
public class BGM : MonoBehaviour
{
    public AudioClip[] clips; // 効果音リスト
    public float minDelay = 3f;
    public float maxDelay = 10f;
    public Vector3 areaSize = new Vector3(10f, 2f, 10f); // 音が鳴る範囲

    void Start()
    {
        StartCoroutine(PlayRandomClips());
    }

    IEnumerator PlayRandomClips()
    {
        while (true)
        {
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            if (clips.Length > 0)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                Vector3 randomPos = transform.position + new Vector3(
                    Random.Range(-areaSize.x / 2, areaSize.x / 2),
                    Random.Range(-areaSize.y / 2, areaSize.y / 2),
                    Random.Range(-areaSize.z / 2, areaSize.z / 2)
                );

                GameObject tempAudio = new GameObject("TempAudio");
                tempAudio.transform.position = randomPos;

                AudioSource source = tempAudio.AddComponent<AudioSource>();
                source.clip = clip;
                source.spatialBlend = 1.0f; // 3Dサウンド
                source.minDistance = 1f;
                source.maxDistance = 15f;
                source.Play();

                Destroy(tempAudio, clip.length + 1f); // 再生後に削除
            }
        }
    }
}