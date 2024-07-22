using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine;

public class DecreasedVisionMachine : InteractionMachine
{
    [HideInInspector] public DecreasedVision decreasedVision;

    private void Start()
    {
        interactionMap.Add(Tag.Criminal, InteractWithCriminal);
        interactionMap.Add(Tag.FireFighter, InteractWithFireFighter);
    }

    private void InteractWithCriminal(GameObject other, bool isEnter)
    {
        if (!IsServer) return;
        if(isEnter) EnterDecreasedVisionCriminal(other);
    }

    private void InteractWithFireFighter(GameObject other, bool isEnter)
    {
        if (!IsServer) return;
        if (isEnter) EnterDecreasedVisionFireFighter(other);
    }

    private void EnterDecreasedVisionCriminal(GameObject other)
    {
        var fireFighterObject = DataManager.Instance.fireFighter?.transform.parent.gameObject;
        if (fireFighterObject != null)
        {
            var networkObject = fireFighterObject.GetComponent<NetworkObject>();

            FindSpecificClient(networkObject);
            //if (networkObject != null)
            //{
            //    Debug.Log($"Sending RPC to FireFighter with Client ID: {networkObject.OwnerClientId}");
            //    ActivateDistractionImageClientRpc(networkObject.OwnerClientId, new ClientRpcParams
            //    {
            //        Send = new ClientRpcSendParams
            //        {
            //            TargetClientIds = new List<ulong> { networkObject.OwnerClientId }
            //        }
            //    });
            //}
        }
        Destroy(this.gameObject);
    }

    private void EnterDecreasedVisionFireFighter(GameObject other)
    {

        foreach(var criminal in DataManager.Instance?.criminals)
        {
            if(criminal != null)
            {
                var criminalOjbect = criminal.transform.parent.gameObject;
                var networkObject = criminalOjbect.GetComponent<NetworkObject>();

                FindSpecificClient(networkObject);

                //if(networkObject != null)
                //{
                //    ActivateDistractionImageClientRpc(networkObject.OwnerClientId, new ClientRpcParams
                //    {
                //        Send = new ClientRpcSendParams
                //        {
                //            TargetClientIds = new List<ulong> { networkObject.OwnerClientId }
                //        }
                //    });
                //}
            }
        }
        Destroy(this.gameObject);
    }

    private void FindSpecificClient(NetworkObject networkObject)
    {
        if (networkObject != null)
        {
            Debug.Log($"Sending RPC to Client ID: {networkObject.OwnerClientId}");
            ActivateDistractionImageClientRpc(networkObject.OwnerClientId, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { networkObject.OwnerClientId }
                }
            });
        }
    }

    [ClientRpc]
    private void ActivateDistractionImageClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        DebuffUIController.Instance.ActivateDistractionImages();
    }
}
