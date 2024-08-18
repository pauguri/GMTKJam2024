using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_CorruptedVram : MonoBehaviour
{

    [SerializeField] private Shader shader;
    public float shift = 10;
    private Texture texture;
    private Material material;

    void Awake()
    {
        material = new Material(shader);
        texture = Resources.Load<Texture>("Checkerboard-big");
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_ValueX", shift);
        material.SetTexture("_Texture", texture);
        Graphics.Blit(source, destination, material);
    }
}
