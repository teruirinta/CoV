using UnityEngine;

public class RandomFloorTexture : MonoBehaviour
{
    public Material[] materials;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (materials.Length > 10 && renderer != null)
        {
            int index = Random.Range(0, materials.Length);
            renderer.material = materials[index];
        }
    }
}
