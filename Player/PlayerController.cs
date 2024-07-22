using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Burst.Intrinsics;
using Unity.Services.Lobbies.Models;
using Unity.Mathematics;

public class PlayerController : NetworkBehaviour
{
    //public static PlayerController LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawned;

    [Header("Movement")] 
    public NetworkVariable<float> moveSpeed = new NetworkVariable<float>();
    public Vector3 moveVec;
    public Vector3 lastInputDir = Vector3.zero;

    [Header("Dash")]
    public bool isDash = false;
    public float dashCoolTime = 3.5f;
    public float lastDashTime = 0f;

    [Header("JoyStick")]
    public float lookSensitivity;
    public VariableJoystick joystick;

    [Header("State")]
    private BaseState currentState;
    public SpriteAnimation animation;

    [Header("Visual")]
    public SprayParticle particle; //Fix

    [SerializeField]
    private LayerMask layerMask;
    public Rigidbody rigidbody;
    Transform childtransform; 

    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        if (!IsOwner) return;

        rigidbody = GetComponent<Rigidbody>();
        childtransform = transform.GetChild(0).transform;

        BattleMapManager.Instance.cinemachineCamera.Follow = transform;

        SetState(new IdleState(this));
    }

    public override void OnNetworkSpawn()
    {
        //OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    void Start()
    {
        //PlayerData playerData = GameMultiPlayer.Instance.GetPlayerDataFromClientId(OwnerClientId);

        if(animation != null && IsOwner)
            animation.PlayAnimationServerRpc("FrontIdle", new Vector3(0,0,0));
    }

    private void Update()
    {
        currentState?.OnStateUpdate();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

#if UNITY_ANDROID
            JoyStickMove();
#endif

        currentState?.OnStateFixedUpdate();
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
    public void SetState(BaseState state)
    {
        if (!IsOwner) return;

        currentState?.OnStateExit();
        currentState = state;
        currentState.OnStateEnter();
    }

    // --------------------------------




















    private void Move()
    {
        Vector3 inputDir = new Vector3(InputManager.instance.moveX, 0, InputManager.instance.moveZ);

        if (inputDir == Vector3.zero)
        {
            string idelAnimation = lastInputDir.z > 0 ? "BackIdle" : "FrontIdle";
            //string idelAnimation = transform.GetChild(0).transform.forward.z > 0 ? "BackIdle" : "FrontIdle";

            if(animation != null && (!animation.IsPlaying(idelAnimation)))
            {
                animation.PlayAnimationServerRpc(idelAnimation, lastInputDir);
            }
            particle.StopParticleClientRpc();
            return;
        }

        string animationName = (inputDir.z > 0) ? "BackWalk" : "FrontWalk";

        animation.SwitchFlipServerRpc(inputDir);

        if (animation != null && (!animation.IsPlaying(animationName)))
            animation.PlayAnimationServerRpc(animationName, inputDir);
       
        particle.PlayParticleClientRpc();
        transform.GetChild(0).transform.rotation = Quaternion.LookRotation(inputDir);
        moveVec = inputDir * moveSpeed.Value * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + moveVec);

        lastInputDir = inputDir;
        // SendInputServerRpc(input);
    }


    private void JoyStickMove()
    {
        if (joystick.Horizontal != 0)
        {
            //animator.SetBool(IsWalking, true);
        }
        else
        {
            //animator.SetBool(IsWalking, false);
        }

        var time = NetworkManager.ServerTime.TimeAsFloat;
        moveVec = new Vector3(joystick.Horizontal, 0, joystick.Vertical) * moveSpeed.Value * Time.deltaTime * 0.01f;

        rigidbody.MovePosition(rigidbody.position + moveVec);

        if (moveVec.sqrMagnitude == 0)
            return;

        Quaternion dirQuat = Quaternion.LookRotation(new Vector3(joystick.Horizontal, 0, joystick.Vertical));
        Quaternion Rot = Quaternion.Slerp(rigidbody.rotation, dirQuat, lookSensitivity);
        rigidbody.MoveRotation(Rot);
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
