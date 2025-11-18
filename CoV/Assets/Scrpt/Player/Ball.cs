using UnityEngine;

public class Ball : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("‚Ô‚Â‚©‚Á‚½‘ŠŽè: " + collision.gameObject.name);
        Destroy(gameObject);
    }
}