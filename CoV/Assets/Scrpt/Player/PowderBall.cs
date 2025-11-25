using UnityEngine;

public class PowderBall : MonoBehaviour
{
    public ParticleSystem hitEffect;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ï≤Ç™ " + collision.gameObject.name + " Ç…ìñÇΩÇ¡ÇΩÅI");

        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect.gameObject, transform.position, Quaternion.identity);
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }

        Destroy(gameObject);
    }
}
