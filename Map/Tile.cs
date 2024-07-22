using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI.Table;
using GoogleSheet.Core.Type;
using DG.Tweening;

public enum TileState
{
    NORAML        = 0,
    ON_CRIMINAL = 1,
    IS_BURNING    = 2,
    IS_BURNDOWN   = 4,
    IS_WET   = 8,
    ON_SAND     = 16,
}
public struct DirValue
{
    public int x;
    public int y;

    public DirValue(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}
[UGS(typeof(DirectionType))]
public enum DirectionType
{
    All,
    Cardinal,
    None
}

public class Tile : NetworkBehaviour
{
    public TileInteractionMachine interactionMachine;

    public NetworkVariable<TileState> state = new(0);

    public HashSet<Job> onTileplayers = new();
    public TileObject onTileObject = null;

    public Vector3Int pos;
    public bool isObstacle;

    List<GameObject> neighbors;

    private DirValue[] dirs = new DirValue[9]
    {
        new DirValue(0, 1),
        new DirValue(0, -1),
        new DirValue(-1, 0),
        new DirValue(1, 0),
        new DirValue(1, 1),
        new DirValue(-1, 1),
        new DirValue(1, -1),
        new DirValue(-1, -1),
        new DirValue(0, 0)
    };

    private void Start()
    {
        Init();       
    }

    private void Init()
    {
        if(TryGetComponent<TileInteractionMachine>(out var machine))
        {
            interactionMachine = machine;
            interactionMachine.tile = this;
        }

        if (!IsServer) return;
        state.OnValueChanged += SetBurningTileCount;

        state.Value = TileState.NORAML;

        pos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
    }


    private void SetBurningTileCount(TileState pre, TileState cur)
    {
        if((pre & TileState.IS_BURNING) != (cur & TileState.IS_BURNING))
        {
            if ((cur & TileState.IS_BURNING) == 0)
            {
                GameStateController.instance.BurningTileCount.Value--;
            }
            else
            {
                GameStateController.instance.BurningTileCount.Value++;
            }
        }
    }

    public List<GameObject> ExploreTile(int x, int y, List<List<GameObject>> allTiles, DirectionType directionType) //int x, int y, GameObject[,] allTiles, DirectionType directionType
    {
        neighbors = new List<GameObject>();
        int rows = allTiles.Count;
        int cols = allTiles[0].Count;

        foreach (var dir in dirs)
        {
            bool include = false;
            switch(directionType)
            {
                case DirectionType.All:
                    include = true;
                    break;
                case DirectionType.Cardinal:
                    include = (dir.x == 0 || dir.y == 0);
                    break;
            }

            if(include)
            {
                int newX = x + dir.x;
                int newY = y + dir.y;

                if(newX >= 0 && newX < rows && newY >= 0 && newY < cols)
                {
                    //GameObject neighborPos = allTiles[x + dir.x, y + dir.y];
                    GameObject neighborPos = allTiles[newX][newY];

                    if (neighborPos != null)
                        neighbors.Add(neighborPos);
                }
            }
        }
        return neighbors;
    }

    // param "ob" : use to subsribe to OnExtinguish event in Fire Class (Not used)
    public void ClearOnTileItem(GameObject ob)
    {
        onTileObject = null;      
    }

    public void Ignite()
    {
        if ((state.Value & TileState.IS_BURNING) == 0)
        {
            state.Value |= TileState.IS_BURNING;

            var firePos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

            var netOb = NetworkObjectPoolLegacy.Singleton.Spawn("Fire", firePos, Quaternion.identity);
            netOb.transform.localScale = Vector3.zero;
            netOb.Spawn(true);

            netOb.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);

            var fire = netOb.GetComponent<Fire>();
            onTileObject = fire;
            fire.tile = this;
        }
    }

    public void Extinguish()
    {
        if (onTileObject is Fire fire)
        {
            state.Value &= ~TileState.IS_BURNING;
            onTileObject = null;
            fire.Extinguish();
        }
    }
}
