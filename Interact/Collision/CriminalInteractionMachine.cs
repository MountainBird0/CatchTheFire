using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.Netcode;
using UnityEngine;

public class CriminalInteractionMachine : InteractionMachine
{
    public Criminal criminal;

    private HashSet<GameObject> currentFires = new();

    private void Start()
    {
        interactionMap.Add(Tag.Tile, InteractWithTile);
        interactionMap.Add(Tag.Fire, InteractWithFire);
        interactionMap.Add(Tag.Item, InteractWithItem);
    }

    #region Tile
    public void InteractWithTile(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterTile(other);
        else ExitTile(other);
    }
    private void EnterTile(GameObject other)
    {
        if (other.TryGetComponent<Tile>(out var tile))
        {
            tile.onTileplayers.Add(criminal);
        }
    }
    private void ExitTile(GameObject other)
    {
        if (other.TryGetComponent<Tile>(out var tile))
        {
            if(tile.onTileplayers.Contains(criminal))
            {
                tile.onTileplayers.Remove(criminal);
            }
        }
    }
    #endregion

    #region Fire
    public void InteractWithFire(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterFire(other);
        else ExitFire(other);
    }
    private void EnterFire(GameObject other)
    {
        currentFires.Add(other);

        criminal.state.Value |= ECriminalState.IN_FIRE;
        if (other.TryGetComponent<Fire>(out var fire))
        {
            fire.OnExtinguish += ExitFire;
        }
    }
    private void ExitFire(GameObject other)
    {
        if(currentFires.Contains(other))
        {
            currentFires.Remove(other);
        }

        if (other.TryGetComponent<Fire>(out var fire))
        {
            fire.OnExtinguish -= ExitFire;

            if (currentFires.Count != 0) return;

            criminal.state.Value &= ~ECriminalState.IN_FIRE;
        }
    }
    #endregion

    #region Item
    private void InteractWithItem(GameObject other, bool isEnter)
    {
        if (!IsOwner) return;
        if (isEnter) EnterItem(other);
    }

    private void EnterItem(GameObject other)
    {
        if (criminal.totalCount != 0) return;

        criminal.totalCount++;
        //Type itemType = GetItemInformation(other);

        //if (criminal.itemSlot.totalCount != 0)
        //{
        //    criminal.itemSlot.totalCount++;
        //}

        Debug.Log($"Criminal EnterItem {other.name}");
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        criminal.itemName = other.name;
        ItemSlot.Instance.AddItemServerRpc(clientId, other.name, criminal.gameObject.GetComponentInParent<NetworkObject>());

        DestroyServerRpc(other.GetComponentInParent<NetworkObject>());

        //criminal.itemSlot.item = (TileObject)Activator.CreateInstance(itemType)    
    }

    [ServerRpc]
    private void DestroyServerRpc(NetworkObjectReference netObRef)
    {
        if (netObRef.TryGet(out NetworkObject netOb))
        {
            Destroy(netOb.gameObject);
        }
    }
    #endregion
}
