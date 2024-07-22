using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;

    public float moveSpeed = 10f;

    public float zoomSpeed = 10.0f; 
    public float minZoom = 5.0f; 
    public float maxZoom = 50.0f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0.0f)
        {
            float newZoom = cam.orthographicSize - scrollInput * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }

}
