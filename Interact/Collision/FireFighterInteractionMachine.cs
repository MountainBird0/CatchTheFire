using CharacterStatus;
using System.Collections.Generic;
using System.Xml;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FireFighterInteractionMachine : InteractionMachine
{
    public FireFighter fireFight;
    public int fireStepValue = 0;

    HashSet<GameObject> currentFire = new();


    private void Start()
    {
        interactionMap.Add(Tag.Tile, InteractWithTile);
        //interactionMap.Add(Tag.Fire, InteractWithFire);
        //interactionMap.Add(Tag.Capsule, InteractWithCapsule);
        interactionMap.Add(Tag.Item, InteractWithItem);
    }

    public void InteractWithTile(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterTile(other);
            else ExitTile(other);
    }

    public void InteractWithFire(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterFire(other);
            else ExitFire(other);
        
    }

    public void InteractWithCapsule(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterCapsule(other);
    }

    private void EnterTile(GameObject other)
    {
        if (other.TryGetComponent<Tile>(out var tile))
        {
            tile.onTileplayers.Add(fireFight);
            tile.state.OnValueChanged += OnTileStateChange;

            if ((tile.state.Value & TileState.IS_BURNING) != 0)
            {
                
                fireFight.controller.moveSpeed.Value = fireFight.fireFighterstats.InFire;
            }
            if((tile.state.Value & TileState.IS_WET) != 0)
            {
                //fireFight.condition.resistancePointChangeRate.Value = 20;
            }
        }
    }
    private void ExitTile(GameObject other)
    {
        if (other.TryGetComponent<Tile>(out var tile))
        {
            if (tile.onTileplayers.Contains(fireFight))
            {
                tile.onTileplayers.Remove(fireFight);
            }

            tile.state.OnValueChanged -= OnTileStateChange;

            if ((tile.state.Value & TileState.IS_BURNING) != 0)
            {
                //fireFight.controller.moveSpeed.Value = 1.7f;
            }
            if ((tile.state.Value & TileState.IS_WET) != 0)
            {
                //fireFight.condition.resistancePointChangeRate.Value = fireFight.fireFighterstats.MoveSpeed;
            }
        }
    }

    private void EnterFire(GameObject other)
    {
        fireStepValue = 0;
        currentFire.Add(other);
        fireFight.state.Value |= FireFighterState.IN_FIRE;  
        if (other.TryGetComponent<Fire>(out var fire))
        {
            fire.burnState.OnValueChanged += OnBurnStateChange;
            OnBurnStateChange(fire.burnState.Value, fire.burnState.Value);
            fire.OnExtinguish += fireFight.EscapeEvent;
        }
    }

    private void ExitFire(GameObject other)
    {
        if(currentFire.Contains(other))
        {
            currentFire.Remove(other);
        }

        if (other.TryGetComponent<Fire>(out var fire))
        {
            fire.burnState.OnValueChanged -= OnBurnStateChange;
            fire.OnExtinguish -= fireFight.EscapeEvent;

            if (currentFire.Count != 0) return;
            
            fireFight.state.Value &= ~FireFighterState.IN_FIRE;
        }
        //fireFight.condition.resistancePointChangeRate.Value = fireFight.fireFighterstats.ResistancePointChangeRateUp;
    }

    private void EnterCapsule(GameObject other)
    {
        if(other.TryGetComponent<ExtinguishingCapsule>(out var capsule) && IsServer)
        {
            //if(fireFight.currentCapsule == 0)
            //{
            //    Debug.Log("ĸ�� �浹!");
            //    fireFight.currentCapsule++;
            //}

            //Debug.Log("Capsule Enter");
            //capsule.GetComponent<NetworkObject>().Despawn();

            AddCapsuleClientRpc();
        }
    }

    [ClientRpc]
    private void AddCapsuleClientRpc()
    {
        if (fireFight.currentCapsule == 0)
        {
            fireFight.currentCapsule++;
        }
    }

    private void OnBurnStateChange(BurnState previous, BurnState current)
    {
        //fireStepValue = (int)current + 1;

        fireStepValue = fireFight.fireFighterstats.ResistancePointChangeRate[(int)current + 1];
        
        //fireFight.condition.resistancePointChangeRate.Value = - fireStepValue;
    }

    private void OnTileStateChange(TileState previous, TileState current)
    {
        if((current & TileState.IS_WET) != 0 && (fireFight.state.Value & FireFighterState.IS_DIE) != 0)                
        {
            fireFight.state.Value &= ~FireFighterState.IS_DIE;
            fireFight.state.Value |= FireFighterState.NORMAL;
            //fireFight.condition.resistancePointChangeRate.Value = 20;
        }

        if ((current & TileState.IS_BURNDOWN) != 0 && (fireFight.state.Value & FireFighterState.IS_DIE) != 0)
        {
            fireFight.state.Value &= ~FireFighterState.IS_DIE;
            fireFight.state.Value |= FireFighterState.NORMAL;
            //fireFight.condition.resistancePointChangeRate.Value = fireFight.fireFighterstats.ResistancePointChangeRateUp;
        }
    }

    #region Item
    private void InteractWithItem(GameObject other, bool isEnter)
    {
        if (!IsOwner) return;
        if (isEnter) EnterItem(other);
    }

    private void EnterItem(GameObject other)
    {
        //Type itemType = GetItemInformation(other);

        //if (criminal.itemSlot.totalCount != 0)
        //{
        //    criminal.itemSlot.totalCount++;
        //}

        if (fireFight.totalCount != 0) return;

        fireFight.totalCount++;

        Debug.Log($"Criminal EnterItem {other.name}");
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        fireFight.itemName = other.name;
        ItemSlot.Instance.AddItemServerRpc(clientId, other.name, fireFight.gameObject.GetComponentInParent<NetworkObject>());

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


    //private System.Type GetItemInformation(GameObject other)
    //{
    //    switch (other.gameObject.name)
    //    {
    //        case "Bomb":
    //            return typeof(Bomb);
    //        case "Invincivility":
    //            return typeof(Invincivility);
    //        default:
    //            throw new System.Exception("None Type");
    //    }
    //}



    #endregion
}
