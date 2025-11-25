using UnityEngine;

public class PowderBall : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("‚Ô‚Â‚©‚Á‚½‘Šè: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); // ‘Šè‚à”j‰óI
        }

        Destroy(gameObject); // ©•ª‚à”j‰óI
    }
}