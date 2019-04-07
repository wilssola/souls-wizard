using UnityEngine;

[ExecuteInEditMode]
public class ImageEffectBase : MonoBehaviour {

    public Material Material;

    private void OnRenderImage(RenderTexture Source, RenderTexture Destination)
    {
        Graphics.Blit(Source, Destination, Material);
    }

}
