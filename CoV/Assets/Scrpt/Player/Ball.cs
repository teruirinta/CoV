using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnParticleCollision(GameObject other)
    {
        Debug.Log("パーティクルが " + other.name + " に当たったよ！");
        if (other.CompareTag("Enemy"))
        {

            Destroy(other);
        }

    }
}
