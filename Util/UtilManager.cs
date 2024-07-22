using UnityEngine;

public class UtilManager : MonoBehaviour
{
    public static void InActiveCamera()
    {
        return;
        foreach (var camera in Camera.allCameras)
        {
            camera.enabled = false;
        }
    }
}
