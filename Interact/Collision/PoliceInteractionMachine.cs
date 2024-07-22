using DG.Tweening;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class PoliceInteractionMachine : InteractionMachine
{
    private Tween moveTween;

    public Police police;
    void Start()
    {
        interactionMap.Add(Tag.Criminal, InteractWithCriminal);
        interactionMap.Add(Tag.Sand, InteractWithSand);
    }


    #region Criminal
    public void InteractWithCriminal(GameObject other, bool isEnter)
    {
        if (isEnter) EnterCriminal(other);
        else ExitCriminal(other);
    }
    private void EnterCriminal(GameObject other)
    {
        if (IsServer)
        {
            police.state.Value |= PoliceState.IS_CANCATCH;
        }
        police.target = other.GetComponent<Criminal>();
    }

    private void ExitCriminal(GameObject other)
    {
        if (IsServer)
        {
            police.state.Value &= ~PoliceState.IS_CANCATCH;
            if (police.tryMakeIceHandcuffs != null)
                StopCoroutine(police.tryMakeIceHandcuffs);
        }
        police.target = null;
    }
    #endregion

    public void InteractWithSand(GameObject other, bool isEnter)
    {
        if (!IsOwner) return;

        Debug.Log($"{GetType()} - 모래랑 충돌");
        if (isEnter) EnterSand(other);
        else ExitSand(other);
    }
    private void EnterSand(GameObject other)
    {
        if (other.TryGetComponent<SandBag>(out var sand))
        {
            Debug.Log($"{GetType()} - {InputManager.instance.moveX}, {InputManager.instance.moveZ}");

            if (InputManager.instance.IsMove())
            {
                var dir = other.transform.position - this.transform.position;
                Vector3 targetDirection = Vector3.zero;

                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
                {
                    if (dir.x > 0)
                    {
                        Debug.Log($"{GetType()} - right object.");
                        targetDirection = Vector3.right;
                    }
                    else if (dir.x < 0)
                    {
                        Debug.Log($"{GetType()} - left object.");
                        targetDirection = Vector3.left;
                    }
                }
                else if (Mathf.Abs(dir.x) < Mathf.Abs(dir.z))
                {
                    if (dir.z > 0)
                    {
                        Debug.Log($"{GetType()} - up object.");
                        targetDirection = new Vector3(0, 0, 1);
                    }
                    else if (dir.z < 0)
                    {
                        Debug.Log($"{GetType()} - down object.");
                        targetDirection = new Vector3(0, 0, -1);
                    }
                }

                if (targetDirection != Vector3.zero)
                {
                    NetworkObjectReference otherObjectRef = other.GetComponent<NetworkObject>();
                    MoveServerRpc(otherObjectRef, targetDirection);

                }
            }
        }
    }

    private void ExitSand(GameObject other)
    {
        KillMoveServerRpc();
    }

    [ServerRpc]
    private void MoveServerRpc(NetworkObjectReference otherObjectRef, Vector3 targetDirection)
    {
        if (otherObjectRef.TryGet(out NetworkObject networkObject))
        {
            GameObject other = networkObject.gameObject;
            moveTween?.Kill(); 
            moveTween = other.transform.DOMove(other.transform.position + targetDirection, 2f)
                                       .SetLoops(-1, LoopType.Incremental)
                                       .SetEase(Ease.Linear);
        }
    }

    [ServerRpc]
    private void KillMoveServerRpc()
    {
        moveTween?.Kill();
    }
}