using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Material material;
    private float offset;
    [SerializeField] private float parallaxFactor = 0.001f;
    [SerializeField] private float gameSpeed = 2f;

    void Start()
    {
        // Clone instance của material
        material = Instantiate(GetComponent<Renderer>().material);
        GetComponent<Renderer>().material = material;
    }

    void LateUpdate() // để “ăn khớp” camera
    {
        float speed = gameSpeed * parallaxFactor;
        offset += Time.deltaTime * speed;
        material.SetTextureOffset("_MainTex", Vector2.right * offset);
    }
}