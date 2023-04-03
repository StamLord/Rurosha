using UnityEngine;
using System.IO;

public class ImpostorGenerator : MonoBehaviour
{
    [SerializeField] private int resolution;
    [SerializeField] private float distance;
    [SerializeField] private LayerMask layer;
    [SerializeField] private Camera impostorCamera;
    [SerializeField] private Transform targetObject;
    [SerializeField] private RenderTexture compositeTexture;
    [SerializeField] private bool Bake;

    private void Update() 
    {
        if(Bake)
        {
            GenerateTextures();
            Bake = false;
        }
    }

    public void GenerateTextures()
    {
        impostorCamera.cullingMask = layer;
        impostorCamera.clearFlags = CameraClearFlags.Nothing;

        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                float divisor = resolution - 1;
                Vector3 pos = new Vector3(x / divisor, 0, z / divisor);
                pos = new Vector3(pos.x - pos.z, 0f, -1f + pos.x + pos.z);
                pos.y = 1 - Mathf.Abs(pos.x) - Mathf.Abs(pos.z);
                pos = pos.normalized * distance;

                // Move camera to positions and capture render texture.
                impostorCamera.transform.position = targetObject.position + pos;
                impostorCamera.transform.LookAt(targetObject, Vector3.up);

                // Render into texture atlas.
                var tileSize = 1.0f / resolution;
                impostorCamera.rect = new Rect(tileSize * x, tileSize * z, tileSize, tileSize);
                impostorCamera.targetTexture = compositeTexture;
                impostorCamera.Render();

                

                impostorCamera.targetTexture = null;
            }
        }

        // Save to regular texture
        TextureToFile.SaveRenderTexture(compositeTexture, Path.Combine(Application.dataPath, "Textures", "Impostors", targetObject.name));
    }
}
