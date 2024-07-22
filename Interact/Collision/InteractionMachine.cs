using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum Tag
{
    Criminal,
    Police,
    FireFighter,
    Fire,
    Water,
    IceHandcuffs,
    Sand,
    Capsule,
    Tile,
    FireExtinguishingFluid,
    Item,
}


public class InteractionMachine : NetworkBehaviour
{
    public Dictionary<Tag, UnityAction<GameObject, bool>> interactionMap = new();

    private void OnTriggerEnter(Collider other)
    {
        Interact(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        Interact(other.gameObject, false);      
    }

    private void Interact(GameObject other, bool isEnter)
    {
        //if (IsServer)
        //{

        if (!GameStateController.instance.IsGamePlayingState()) return;
       

        if (Enum.TryParse(other.tag, out Tag tag))
        {
            if (interactionMap.TryGetValue(tag, out var interaction))
            {
                interaction?.Invoke(other.gameObject, isEnter);
            }
        }
        //}
    }
}
