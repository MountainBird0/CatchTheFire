using DG.Tweening;
using UnityEngine;

public class AimController : MonoBehaviour
{
    public VariableJoystick joystick;
    private Vector3 moveVec;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        JoyRot();
    }

    void JoyRot()
    {
        moveVec = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        if (moveVec.sqrMagnitude == 0)
            return;
        Quaternion dirQuat = Quaternion.LookRotation(new Vector3(joystick.Horizontal, 0, joystick.Vertical));
        Quaternion Rot = Quaternion.Slerp(transform.rotation, dirQuat, 0.1f);
        transform.rotation = Rot;
    }
}
