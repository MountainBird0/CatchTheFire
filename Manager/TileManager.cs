using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using System;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Linq;


public class TileManager : NetworkBehaviour
{
    public static TileManager Instance {  get; set; }
    
    private Tile[] allTiles;

    [SerializeField] private List<List<GameObject>> tileObject;

    public Dictionary<Vector3Int, Tile> board = new();

    private Vector3Int[] dirs = new Vector3Int[4]
    {
        new Vector3Int(0,0,1),
        new Vector3Int(0,0,-1),
        new Vector3Int(1,0,0),
        new Vector3Int(-1,0,0),
    };

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        if(IsServer)
        {
            await MapMaker.Instance.test();
            LoadTileInit();
            BattleMapManager.Instance.tiles = tileObject;
            BattleMapManager.Instance.SpawnDecreasedVision(); //Fix
        }
    }
    private void LoadTileInit()
    {
        int maxCol = 0;
        int maxRow = 0;

        foreach (var tile in MapMaker.Instance.loadMapData)
        {
            int col = (int)tile.Key.x;
            int row = (int)tile.Key.z;

            if (col > maxCol) maxCol = col;
            if (row > maxRow) maxRow = row;
        }

        tileObject = new List<List<GameObject>>();
        for (int i = 0; i <= maxCol; i++)
        {
            tileObject.Add(new List<GameObject>());
            for (int j = 0; j <= maxRow; j++)
            {
                tileObject[i].Add(null);
            }
        }

        foreach (var tileData in MapMaker.Instance.loadMapData)
        {
            if (tileData.Value != null)
            {
                GameObject tileGameObject = tileData.Value.gameObject;

                if (tileGameObject.GetComponent<Tile>() == null)
                    tileGameObject.gameObject.AddComponent<Tile>();

                int col = (int)tileData.Key.x;
                int row = (int)tileData.Key.z;

                tileObject[col][row] = tileGameObject;

                Vector3Int pos = new Vector3Int(col, 0, row);
                Tile tile = tileGameObject.gameObject.GetComponent<Tile>();
                board.Add(pos, tile);
            }
        }

        Debug.Log($"{tileObject.Count} Map Load Success");

    }

    public List<GameObject> ExploreNeighborsFromTile(int x, int y, DirectionType directionType)
    {
        List<GameObject> neighbors = new();

        GameObject currentTileObject = tileObject[x][y];
        Tile tileComponent = currentTileObject?.GetComponent<Tile>();

        if(tileComponent != null) 
        {
            neighbors = tileComponent.ExploreTile(x, y, this.tileObject, directionType);            
        }

        return neighbors;
    }

    public List<GameObject> GetTilesInDirection(float startX, float startY, Vector3 direction, int numTiles)
    {
        List<GameObject> tiles = new List<GameObject>();

        int roundsX = Mathf.RoundToInt(startX);
        int roundsY = Mathf.RoundToInt(startY);

        for (int i = 1; i <= numTiles; i++)
        {
            int newX = roundsX + Mathf.RoundToInt(direction.x) * i;
            int newY = roundsY + Mathf.RoundToInt(direction.z) * i;

            if (newX >= 0 && newX < tileObject.Count && newY >= 0 && newY < tileObject[0].Count)
            {
                tiles.Add(tileObject[newX][newY]);
            }
            else
            {
                break;
            }
        }
        return tiles;
    }

    /// <summary>
    /// plyer current location forward tile 
    /// </summary>
    /// <param name="startX"> player position x </param>
    /// <param name="startY"> player position y </param>
    /// <param name="direction"> player forward </param>
    /// <returns> player forward tile </returns>
    public GameObject GetTilesForwardDirection(float startX, float startY, Vector3 direction)
    {
        int reoundX = Mathf.RoundToInt(startX);
        int reoundY = Mathf.RoundToInt(startY);

        int newX = reoundX + Mathf.RoundToInt(direction.x);
        int newY = reoundY + Mathf.RoundToInt(direction.z);

        if (newX >= 0 && newX < tileObject.Count && newY >= 0 && newY < tileObject[0].Count)
        {
            return tileObject[newX][newY];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// player current tile 
    /// </summary>
    /// <param name="startX"> player position x </param>
    /// <param name="startY"> player position z </param>
    /// <returns> current location tile </returns>
    public GameObject GetCurrentTilesPos(float startX, float startY)
    {
        int reoundX = Mathf.RoundToInt(startX);
        int reoundY = Mathf.RoundToInt(startY);

        return tileObject[reoundX][reoundY];
    }

    /// <summary>
    /// player current direction corn tile
    /// </summary>
    /// <param name="inputDir"> rotation gameObject </param>
    /// <param name="currentPos"> player current position </param>
    /// <param name="range"> corn length </param>
    /// <returns> player position corn tile </returns>
    public List<GameObject> GetCornTileDirection(Vector3 inputDir,Vector3 currentPos, int range)
    {
        Vector3 next;

        List<GameObject> tileResult = new();

        int lateral = 1;
        int min = 0;
        int max = 0;

        GameObject currentTile = GetCurrentTilesPos(currentPos.x, currentPos.z);


        for (int i = 1; i < range; i++)
        {
            min = -(lateral/2);
            max = (lateral/2);

            for(int j = min; j <= max; j++)
            {
                next = GetNext(inputDir, currentTile, i, j);
                if (next == null) continue;
                else AddTileData(tileResult, next);

                /*tileResult.Add(tileObject[(int)next.x][(int)next.z]);*/
            }

            lateral += 2;
        }

        return tileResult;
    }
    private Vector3 GetNext(Vector3 inputDir, GameObject currentTile, int agr1, int agr2)
    {
        //Vector3 next = Vector3.zero;
        //next = new Vector3(inputDir.x + agr1, 0, inputDir.z + agr2);

        Vector3 currentPos = new Vector3(currentTile.transform.position.x, 0 , currentTile.transform.position.z);

        Vector3 forwardDir = inputDir.normalized * agr1;
        Vector3 sideDir = Vector3.Cross(inputDir.normalized, Vector3.up).normalized * agr2;

        return currentPos + forwardDir + sideDir;
    }

    private void AddTileData(List<GameObject> obj, Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);

        GameObject tile = tileObject[x][z];
        if(tile != null)
            obj.Add(tile);
    }

    public Tile[] GetAllTiles()
    {
        return allTiles;
    }

    public List<Tile> SearchRange(ERangeType rangeType, Vector3Int startPos, int range)
    {
        ERangeType type = rangeType;

        switch (rangeType)
        {
            case ERangeType.CONSTANT:
                return GetConstantRange(startPos, range);

            case ERangeType.CONE:
                return GetConeRange(startPos, range);

            case ERangeType.LINE:
                return GetLineRange(startPos, range);
        }

        return null;
    }


    #region SearchRange
    private List<Tile> GetConstantRange(Vector3Int startPos, int range)
    {
        List<Tile> tileResult = new();
        tileResult.Add(GetTile(startPos));

        Dictionary<Vector3Int, int> posDic = new();
        posDic.Add(startPos, 0);

        Queue<Vector3Int> checkNow = new Queue<Vector3Int>();
        Queue<Vector3Int> checkNext = new Queue<Vector3Int>();
        Vector3Int now;
        Vector3Int next;

        checkNow.Enqueue(startPos);

        while (checkNow.Count > 0)
        {
            now = checkNow.Dequeue();

            for (int i = 0; i < dirs.Length; i++)
            {
                next = now + dirs[i];

                if (posDic[now] + 1 > range)
                {
                    continue;
                }
                if (!posDic.ContainsKey(next))
                {
                    posDic.Add(next, posDic[now] + 1);
                    checkNext.Enqueue(next);
                }

                if (board.TryGetValue(next, out Tile nextTile) && !tileResult.Contains(nextTile))
                {
                    tileResult.Add(nextTile);
                }
            }

            if (checkNow.Count == 0)
            {
                Queue<Vector3Int> temp = checkNow;
                checkNow = checkNext;
                checkNext = temp;
            }
        }

        return tileResult;
    }

    private List<Tile> GetConeRange(Vector3Int startPos, int range)
    {
        List<Tile> tileResult = new();

        Vector3Int next;

        int lateral = 1;
        int min = 0;
        int max = 0;

        for (int i = 1; i < range; i++)
        {
            min = -(lateral / 2);
            max = (lateral / 2);

            for (int j = min; j <= max; j++)
            {
                next = GetNext(InputManager.instance.lastInputDir.normalized, startPos, i, j);

                if (next == null)
                {
                    continue;
                }
                else
                {
                    var tile = GetTile(next);
                    if (tile == null) continue;
                    tileResult.Add(tile);
                }
            }
            lateral += 2;
        }

        return tileResult;
    }
    private Vector3Int GetNext(Vector3 inputDir, Vector3Int startPos, int agr1, int agr2)
    {
        Vector3 forwardDir = inputDir.normalized * agr1;
        Vector3 sideDir = Vector3.Cross(inputDir.normalized, Vector3.up).normalized * agr2;

        var pos = startPos + forwardDir + sideDir;

        Vector3Int reseult = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);

        return reseult;
    }

    private List<Tile> GetLineRange(Vector3Int startPos, int range)
    {
        List<Tile> tileResult = new();
        tileResult.Add(GetTile(startPos));

        var dir = InputManager.instance.lastInputDir.normalized;

        tileResult.AddRange(GetTilesInDirection(startPos, dir, range));

        tileResult.AddRange(GetTilesInDirection(startPos, -dir, range));

        tileResult = tileResult.OrderBy(tile => Vector3Int.Distance(startPos, tile.pos)).ToList();

        return tileResult;
    }
    private List<Tile> GetTilesInDirection(Vector3Int startPos, Vector3 direction, int range)
    {
        List<Tile> tileResult = new();

        for (int i = 1; i <= range; i++)
        {
            Vector3Int nextPos = startPos + Vector3Int.RoundToInt(direction * i);
            var tile = GetTile(nextPos);
            if (tile == null) break;
            tileResult.Add(tile);
        }

        return tileResult;
    }

    #endregion


    public Tile GetTile(Vector3Int pos)
    {
        Tile tile = null;
        board.TryGetValue(pos, out tile);

        return tile;
    }
}
