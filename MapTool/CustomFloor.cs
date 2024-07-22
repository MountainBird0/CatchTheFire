using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CustomFloor : SingletonComponent<CustomFloor>
{
    //public List<GameObject> tiles = new List<GameObject>();

    //[ShowInInspector]
    public Dictionary<Vector3, MapData> mapDatas = new Dictionary<Vector3, MapData>();

    //[OnInspectorInit]
    //private void InitMapDatas()
    //{
    //    mapDatas = new Dictionary<Vector3, MapData>();
    //}

    //[ShowInInspector]
    //private Dictionary<Vector3, MapData> mapDatas { get; set; }

    public bool IsDataExist(Vector3 cellPos)
    {
        return mapDatas.ContainsKey(cellPos);
    }

    public void AddMapData(Vector3 cellPos, CustomPaletteItem item)
    {
        var target = GameObject.Instantiate(item.tileObject, transform);
        target.transform.SetParent(transform);
        target.transform.position = cellPos;


        var map = target.AddComponent<MapData>();
        map.id = item.id;   
        map.cellPos = cellPos;

        mapDatas.Add(cellPos, map);

        //return map;
    }

    public void RemoveMapData(Vector3 cellPos)
    {
        DestroyImmediate(mapDatas[cellPos].gameObject);
        mapDatas.Remove(cellPos);
    }

    public Dictionary<Vector3, MapData> CreateMapBoundary()
    {
        if (mapDatas.Count == 0) return null;

        HashSet<Vector3> boundary = new();

        foreach(var mapData in mapDatas.Keys)
        {
            Vector3[] neighbors = new Vector3[]
            {
                mapData + Vector3.left,
                mapData + Vector3.right,
                mapData + Vector3.forward,
                mapData + Vector3.back,
            };

            foreach(var neighbor in neighbors)
            {
                if(!mapDatas.ContainsKey(neighbor))
                {
                    boundary.Add(neighbor);
                    break;
                }
            }
        }

        foreach(var mp in boundary)
        {
            MapData md = new MapData();
            md.id = 99;
            md.cellPos = mp;
            mapDatas.Add(mp, md);
        }

        return mapDatas;
    }

}
