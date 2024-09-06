using UnityEngine;

[ExecuteInEditMode]
public class UnderwaterEffect : MonoBehaviour
{
    public Material underwaterMaterial;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (underwaterMaterial != null)
        {
            Graphics.Blit(src, dest, underwaterMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}