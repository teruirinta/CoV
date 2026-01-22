using UnityEngine;

public class RandomAmbientSFX : MonoBehaviour
{
    public AudioClip[] ambientClips;
    public float minDelay = 5f;
    public float maxDelay = 20f;
    public float minVolume = 0.3f;
    public float maxVolume = 1.0f;
    public float fadeDuration = 2f; // フェードイン・アウトの時間

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        StartCoroutine(PlayAmbientSounds());
    }

    System.Collections.IEnumerator PlayAmbientSounds()
    {
        while (true)
        {
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            if (!audioSource.isPlaying && ambientClips.Length > 0)
            {
                AudioClip clip = ambientClips[Random.Range(0, ambientClips.Length)];
                float targetVolume = Random.Range(minVolume, maxVolume);

                audioSource.clip = clip;
                audioSource.volume = 0f;
                audioSource.Play();

                // フェードイン
                yield return StartCoroutine(FadeVolume(targetVolume, fadeDuration));

                // 再生が終わるまで待つ（フェードアウト分を引いておく）
                yield return new WaitForSeconds(clip.length - fadeDuration);

                // フェードアウト
                yield return StartCoroutine(FadeVolume(0f, fadeDuration));

                audioSource.Stop();
            }
        }
    }

    System.Collections.IEnumerator FadeVolume(float targetVolume, float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
