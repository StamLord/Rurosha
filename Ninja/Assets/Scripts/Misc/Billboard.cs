using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera camera;

    private void LateUpdate()
    {
        if(camera == null)
            camera = Camera.main;
        
        transform.LookAt(camera.transform);
        transform.Rotate(0, 180, 0);
    }
}
