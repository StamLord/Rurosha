using UnityEngine;

public class TextureToFile : MonoBehaviour
{
    static public void SaveTexture(Texture2D texture, string filePath)
    {
        System.IO.File.WriteAllBytes(filePath + ".png", texture.EncodeToPNG());
    }

    static public void SaveRenderTexture(RenderTexture renderTexture, string filePath)
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false, false);
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = renderTexture;

        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height),0 ,0);
        texture.Apply();

        RenderTexture.active = old;

        SaveTexture(texture, filePath);

        if (Application.isPlaying)
           Object.Destroy(texture);
        else
           Object.DestroyImmediate(texture);
    }
}
