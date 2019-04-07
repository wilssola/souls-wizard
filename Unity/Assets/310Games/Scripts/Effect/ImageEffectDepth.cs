using UnityEngine;

[ExecuteInEditMode]
public class ImageEffectDepth : MonoBehaviour {

    public Material Material;

    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture Source, RenderTexture Destiny)
    {
        Graphics.Blit(Source, Destiny, Material);
    }

}