using Unity.Netcode;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private string moveXName = "Horizontal";
    private string moveZName = "Vertical";

    public float moveX { get; private set; }
    public float moveZ { get; private set; }

    public Vector3 lastInputDir = Vector3.zero;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        moveX = Input.GetAxis(moveXName);
        moveZ = Input.GetAxis(moveZName);

        if(IsMove())
        {
            lastInputDir = new Vector3(moveX, 0, moveZ);
        }
    }

    public float MoveValue()
    {
        return Mathf.Abs(moveX) + Mathf.Abs(moveZ);
    }

    public bool IsMove()
    {
        if (moveX == 0 && moveZ == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
