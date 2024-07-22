using UnityEngine;

public class A3DBillboard : MonoBehaviour
{
    public enum A3DBillboardType
    {
        Flat,      // Inverse camera rotation
        Sphere,    // Looks directly at camera
        Axial      // Swivels around defined axis
    }

    //*** INSPECTOR FIELDS ***

    [SerializeField] private Vector3 vAxis = Vector3.up;                                   // Axis for arbitrary rotation
    [SerializeField] private A3DBillboardType billboard = A3DBillboardType.Sphere;         // Test if pitch control is clamped
    [SerializeField] private bool correctRoll = true;                                      // Correct for camera roll

    //*** LOCAL VARIABLES ***
    private Vector3 vLook;                  // constructed billboard forward vector
    private Vector3 vRight;                 // constructed billboard right vector
    private Vector3 vUp;                    // constructed billboard up vector

    private Transform billboardTransform;   // link to billboard object transform
    private Transform cameraTransform;      // link to camera object transform


    // Use this for initialization
    void Start()
    {
        billboardTransform = this.transform;
        cameraTransform = BattleMapManager.Instance.cinemachineCamera.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (billboard == A3DBillboardType.Flat)
        {
            // Flat billboards opposite rotation to camera
            billboardTransform.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);

            // Check if correct for camera roll
            if (correctRoll)
            {
                Vector3 CameraRotation = cameraTransform.eulerAngles;
                if (CameraRotation.z != 0) { billboardTransform.Rotate(0f, 0f, -CameraRotation.z); }
            }
        }
        else if (billboard == A3DBillboardType.Sphere)
        {
            // Spherical billboards look at camera position
            //MyTransform.LookAt(MyCameraTransform.position, MyCameraTransform.up);
            //MyTransform.Rotate(new Vector3(0f, 180f, 0f));


            // create billboard look vector
            vLook = billboardTransform.position - cameraTransform.position;
            vLook.Normalize();

            // create billboard right vector
            vRight = Vector3.Cross(cameraTransform.up, vLook);

            // create billboard up vector
            vUp = Vector3.Cross(vLook, vRight);

            // spherical billboard with look direction towards camera
            billboardTransform.rotation = Quaternion.LookRotation(vLook, vUp);

            // Check if correct for camera roll
            if (correctRoll)
            {
                Vector3 CameraRotation = cameraTransform.eulerAngles;
                if (CameraRotation.z != 0) { billboardTransform.Rotate(0f, 0f, -CameraRotation.z); }
            }
        }
        else if (billboard == A3DBillboardType.Axial)
        {
            //create temporary billboard look vector
            vLook = billboardTransform.position - cameraTransform.position;
            vLook.Normalize();

            //create billboard right vector
            float visible = Mathf.Abs(Vector3.Dot(vAxis, vLook));
            if (visible >= 1)
            {
                // look vector is parallel to axis
                vLook = vAxis;
            }
            else
            {
                // create and normalize right vector
                vRight = Vector3.Cross(vAxis, vLook);
                vRight.Normalize();

                // create final billboard look vector
                vLook = Vector3.Cross(vRight, vAxis);

                // create billboard up vector
                vUp = Vector3.Cross(vLook, vRight);

                // axial billboard with look rotation axis aligned
                billboardTransform.rotation = Quaternion.LookRotation(vLook, vUp);
            }
        }
    }
}