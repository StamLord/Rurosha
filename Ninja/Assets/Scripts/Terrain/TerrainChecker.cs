using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChecker : MonoBehaviour
{
    public struct TextureCoords
    {
        public int x;
        public int z;
    }

    public static TextureCoords ConvertPosition(Vector3 position, Terrain terrain)
    {
        Vector3 terrainPos = position - terrain.transform.position;
        Vector3 mapPos = new Vector3 (
            terrainPos.x / terrain.terrainData.size.x, 0,
            terrainPos.z / terrain.terrainData.size.z);

        int x = (int)(mapPos.x * terrain.terrainData.alphamapWidth);
        int z = (int)(mapPos.z * terrain.terrainData.alphamapHeight);

        return new TextureCoords
        {
            x = x,
            z = z
        };
    }

    public static float[] SampleTextures(TextureCoords coords, Terrain terrain)
    {
        float[,,] aMap = terrain.terrainData.GetAlphamaps (coords.x, coords.z, 1, 1);

        float[] textureValues = new float[terrain.terrainData.alphamapLayers];

        for (int i = 0; i < textureValues.Length; i++)
            textureValues[i] = aMap[0,0,i];

        return textureValues;
    }

    public static int[,] SampleDetailLayer(TextureCoords coords, Terrain terrain, int layer)
    {
        int[,] dMap = terrain.terrainData.GetDetailLayer (coords.x, coords.z, 20, 20, layer);


        for (int i = 0; i < 20; i++)
        {
           for (int j = 0; j < 20; j++)
           {
                Debug.Log(dMap[i,j]);       
           } 
        }
        

        return dMap;
    }
}
