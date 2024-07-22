using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : NetworkBehaviour
{
    public static ItemSlot Instance;

    public Job job;

    [SerializeField] public Button button;
    [SerializeField] private TMP_Text text;
    public TileObject item;
    public string itemName;
    private Skill skill;

    public int totalCount = 0;

    private void Awake()
    {
        Instance = this;

        Init();
    }

    private void Init()
    {
        //button = GetComponent<Button>();

        button.gameObject.SetActive(false);
        // button.onClick.AddListener(() => UseItem());
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddItemServerRpc(ulong clientId, string _itemName, NetworkObjectReference _job)
    {
        //if (totalCount == 0)
        //{
        //   totalCount++;
        itemName = _itemName.Replace("(Clone)", "");
        //text.text = itemName;
        if (_job.TryGet(out NetworkObject networkObject))
        {
            job = networkObject.gameObject.GetComponentInChildren<Job>();
            Debug.Log($" LocalClient >> {job}");
            Debug.Log($" LocalClient >> {NetworkManager.Singleton.LocalClientId}");
            AddItemClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { clientId } } });
        }
        //}
    }

    [ClientRpc]
    private void AddItemClientRpc(ClientRpcParams clientRpcParams = default)
    {
        // if(NetworkManager.Singleton.LocalClientId == clientId)
        //{
        ActivateButton();
        //}
    }


    private void ActivateButton()
    {
        button.gameObject.SetActive(true);
    }

    //public void AddItem(string _itemName, Job _job)
    //   {
    //       if(totalCount == 0)
    //       {
    //           totalCount++;
    //           itemName = _itemName.Replace("(Clone)", "");
    //           //text.text = itemName;
    //           job = _job;


    //           NetworkObject networkObject = job.GetComponentInParent<NetworkObject>();

    //           if(networkObject != null)

    //           AddItemClientRpc(FindLocalClientId(networkObject));
    //       }
    //   }

    //private ClientRpcParams FindLocalClientId(NetworkObject networkObject)
    //{
    //    ulong clientId = networkObject.OwnerClientId;

    //    ClientRpcParams rpcParams = new ClientRpcParams
    //    {
    //        Send = new ClientRpcSendParams
    //        {
    //            TargetClientIds = new ulong[] { clientId }
    //        }
    //    };
    //    return rpcParams;
    //}


    //[ClientRpc]
    //private void AddItemClientRpc(ClientRpcParams clientRpcParams = default)
    //[ClientRpc]
    //private void AddItemClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
    //private ClientRpcParams FindLocalClientId(NetworkObject networkObject)
    //{
    //    Debug.Log($"AddItemClientRpc called on client {NetworkManager.Singleton.LocalClientId}");

    //    ActivateButton();

    //    if(NetworkManager.Singleton.LocalClientId == clientId)
    //    {
    //        ActivateButton();
    //    }
    //    ulong clientId = networkObject.OwnerClientId;

    //    ClientRpcParams rpcParams = new ClientRpcParams
    //    {
    //        Send = new ClientRpcSendParams
    //        {
    //            TargetClientIds = new ulong[] { clientId }
    //        }
    //    };
    //    return rpcParams;
    //}


    //[ClientRpc]
    //private void AddItemClientRpc(ClientRpcParams clientRpcParams = default)
    //{
    //    Debug.Log($"AddItemClientRpc called on client {NetworkManager.Singleton.LocalClientId}");

    //    ActivateButton();

    //}


    //private void ActivateButton()
    //{
    //    button.gameObject.SetActive(true);
    //}

    //private void UseItem()
    //{
    //    Type jobType = job.GetType();

    //    var method = jobType.GetMethod("GetSkillData", BindingFlags.Public | BindingFlags.Instance);

    //    totalCount--;

    //    if(itemName == "Bomb")
    //    {
    //        //var genericMethod = method.MakeGenericMethod(typeof(int));

    //        method.Invoke(job, new object[] { 2 });
    //    }
    //    else if(itemName == "Invincivility")
    //    {

    //    }

    //    button.gameObject.SetActive(false);
    //}

}
