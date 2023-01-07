using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Screenshot : MonoBehaviour
{
    [SerializeField] private KeyCode screenshotKey = KeyCode.F12;
    [SerializeField] private string filePath = "Screenshots";
    void Update()
    {
        if(Input.GetKeyDown(screenshotKey))
            TakeScreenShot();
    }

    private void TakeScreenShot()
    {
        Debug.Log("Screenshot take");
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false, false);
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);

        string name = "Screenshot " + DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss");
        TextureToFile.SaveTexture(texture, Path.Combine(Application.dataPath, filePath, name));
    }   
}
