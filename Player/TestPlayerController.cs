using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Burst.Intrinsics;
using Unity.Services.Lobbies.Models;
using Unity.Mathematics;
using static UnityEngine.Rendering.DebugUI.Table;


public class TestPlayerController : NetworkBehaviour
{
    //public static PlayerController LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawned;


    [Header("Movement")]

    float moveSpeed;
    //public float moveSpeed;
    private Vector2 curMovementInput;
    Transform childtransform;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public VariableJoystick joystick;
    private Vector3 moveVec;
    [SerializeField]
    private LayerMask layerMask;

    Vector3 currentDirection;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    private Animator animator;


    public float fPushTime = 0;

    private Rigidbody rigidbody;


    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        childtransform = transform.GetChild(0).transform;
    }

    public override void OnNetworkSpawn()
    {
        //OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    void Start()
    {
        //PlayerData playerData = GameMultiPlayer.Instance.GetPlayerDataFromClientId(OwnerClientId);

        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //Init();
    }

    private void FixedUpdate()
    {

#if UNITY_ANDROID
            // JoyStickMove();
#endif
        Move(curMovementInput);


    }
    private void LateUpdate()
    {
        if (Input.GetKey("escape"))
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
             Application.Quit(); 
#endif
        }

    }
    //public void OnLookInput(InputAction.CallbackContext context)
    //{
    //    mouseDelta = context.ReadValue<Vector2>();
    //}

    //public void OnMoveInput(InputAction.CallbackContext context)
    //{
    //        if (context.phase == InputActionPhase.Performed)
    //        {
    //            curMovementInput = context.ReadValue<Vector2>();
    //            SetCurrentDirection(context.ReadValue<Vector2>());
    //            animator.SetBool(IsWalking, true);
    //        }
    //        else if (context.phase == InputActionPhase.Canceled)
    //        {
    //            curMovementInput = Vector2.zero;
    //            SetCurrentDirection(Vector2.zero);
    //            animator.SetBool(IsWalking, false);
    //        }
    //}

    private void Move(Vector2 input)
    {
        SendInput(input);
    }

    public void SendInput(Vector2 vec)
    {
        curMovementInput = vec.normalized;
        moveSpeed = 2f;
        //if (SetCurrentDirectionRay())
        //    return;

        if (curMovementInput.sqrMagnitude == 0)
            return;

        Quaternion dirQuat = Quaternion.LookRotation(new Vector3(curMovementInput.x, 0, curMovementInput.y));
        Quaternion Rot = Quaternion.Slerp(transform.GetChild(0).transform.rotation, dirQuat, lookSensitivity);

        transform.GetChild(0).transform.rotation = Rot;
        
        moveVec = new Vector3(curMovementInput.x, 0, curMovementInput.y) * moveSpeed * Time.deltaTime;

        rigidbody.MovePosition(rigidbody.position + moveVec);

    }
    public void SetCurrentDirection(Vector2 direction)
    {
        currentDirection.Set(direction.x, 0, direction.y);
    }
    public bool SetCurrentDirectionRay()
    {
        Ray[] rays = new Ray[3]
        {
         new Ray(childtransform.position +  (childtransform.right * -0.2f) + (childtransform.up * 0.5f)+(childtransform.forward * 0.3f), childtransform.forward),
         new Ray(childtransform.position +  (childtransform.right * +0.2f) + (childtransform.up * 0.5f) +(childtransform.forward * 0.3f), childtransform.forward),
         new Ray(childtransform.position +  (childtransform.up * 0.5f)+(childtransform.forward * 0.3f), childtransform.forward),
         };
        RaycastHit hit;
        for (int i = 0; i < rays.Length; i++)
        {
            Debug.DrawRay(rays[i].origin, rays[i].direction * 0.1f, Color.red);
        }
        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], out hit, 0.1f, layerMask))
            {
                Debug.Log(hit.transform.gameObject.ToString());
                return true;
            }
        }
        return false;
    }
}
