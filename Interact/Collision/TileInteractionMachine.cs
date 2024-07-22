using System;
using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class TileInteractionMachine : InteractionMachine
{
    public Tile tile;

    private Coroutine tryMakeFire; // Todo : Change name

    private void Start()
    {
        interactionMap.Add(Tag.Criminal, InteractWithCriminal);
        interactionMap.Add(Tag.Sand, InteractWithSand);
    }

    #region Criminal
    public void InteractWithCriminal(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterCriminal(other);
        else ExitCriminal(other);
    }
    private void EnterCriminal(GameObject other)
    {
        if(other.TryGetComponent<Criminal>(out var criminal))
        {
            if ((criminal.state.Value & ECriminalState.IS_STUNNED) != 0) return;

            tile.state.Value |= TileState.ON_CRIMINAL;
            //if (tile.onTileObject == null && tryMakeFire == null)
            //{
            //    tryMakeFire = StartCoroutine(TryMakeFire());
            //}

            //if((tile.state.Value & TileState.IS_BURNING) != 0)
            //{
            //    tile.condition.hpChangeRate.Value = -2f;
            //}
        }
    }
    private void ExitCriminal(GameObject other)
    {
        tile.state.Value &= ~TileState.ON_CRIMINAL;
        if (tryMakeFire != null)
        {
            StopCoroutine(tryMakeFire);
            tryMakeFire = null;
        }

        //if((tile.state.Value & TileState.IS_BURNING) == 0)
        //{
        //    tile.condition.hpChangeRate.Value = 0f;
        //}
        //else
        //{
        //    tile.condition.hpChangeRate.Value = -1f;
        //}
    }

    // Todo : invoke other place
    private IEnumerator TryMakeFire()
    {
        yield return new WaitForSeconds(0.1f);

        if ((tile.state.Value & TileState.ON_CRIMINAL) == 0) yield return null;

        if ((tile.state.Value & TileState.IS_BURNDOWN) == 0
            && (tile.state.Value & TileState.IS_WET) == 0)
        {
            MakeFire();
            // tile.condition.hpChangeRate.Value = -2f;
        }
    }

   
    public void MakeFire()
    {
        if ((tile.state.Value & TileState.IS_BURNDOWN) != 0) return;
        if ((tile.state.Value & TileState.IS_WET) != 0) return;
        if ((tile.state.Value & TileState.ON_SAND) != 0) return;

        var pos = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);

        //var netOb = NetworkObjectPool.Instance.GetFromPool("Fire", pos, Quaternion.identity);
        //netOb.Spawn(true);

        var netOb = NetworkObjectPoolLegacy.Singleton.Spawn("Fire", pos, Quaternion.identity);
        netOb.Spawn(true);

        var fire = netOb.GetComponent<Fire>();
        fire.OnExtinguish += tile.ClearOnTileItem;
        tile.onTileObject = fire;
        fire.tile = tile;

        tile.state.Value |= TileState.IS_BURNING;
        //fire.condition.firePointChangeRate.Value = fire.stats.hpChangeRate;
        Debug.Log($"fire");

        // 
        // MakeFireClientRpc();

    }
    [ClientRpc] // Todo : check really need rpc
    public void MakeFireClientRpc()
    {
        // tile.condition.hp.gameObject.SetActive(true);
    }
    #endregion

    #region Sand
    public void InteractWithSand(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterSand(other);
        else ExitSand(other);
    }

    private void ExitSand(GameObject other)
    {
        tile.state.Value |= TileState.ON_SAND;
    }

    private void EnterSand(GameObject other)
    {
        tile.state.Value &= ~TileState.ON_SAND;
    }
    #endregion
}
