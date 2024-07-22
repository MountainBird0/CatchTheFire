using GoogleSheet;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UGS;
using UnityEditor;
using UnityEngine;

public class GoogleSheetDataReader : SingletonComponent<GoogleSheetDataReader>
{
    private List<CustomPaletteItem> items;
    private Queue<object> qMapData = new();
    private bool isProcessing;

    public EMapInformation currentMapInfor;

    public void SetItemPaletteItem(List<CustomPaletteItem> _items)
    {
        this.items = _items;
    }

    public async void WriteMapDataToGoogle(Dictionary<Vector3, MapData> mapDatas)
    {
        QueueEnqueue(mapDatas);
        asyncWriteDate();
        Debug.Log("GoogleSheetData >> Write Success!");
    }

    private System.Type GetSaveMapType()
    {
        switch (currentMapInfor)
        {
            case EMapInformation.MAP1:
                return typeof(MapInformation.Data1);
            case EMapInformation.MAP2:
                return typeof(MapInformation.Data2);
            case EMapInformation.MAP3:
                return typeof(MapInformation.Data3);
            case EMapInformation.MAP4:
                return typeof(MapInformation.Data4);
            default:
                throw new System.Exception("None Type");
        }
    }


    private async void QueueEnqueue(Dictionary<Vector3, MapData> mapDatas)
    {
        int i = 0;

        foreach (var mapData in mapDatas)
        {
            //var newData = new MapInformation.Data();

            var newData = Activator.CreateInstance(GetSaveMapType());
            Vector3 cellPos = mapData.Key;
            MapData map = mapData.Value;

            //newData.index = i++;
            //newData.CellPosX = cellPos.x;
            //newData.CellPosY = cellPos.y;
            //newData.CellPosZ = cellPos.z;
            //newData.TileID = map.id;

            SetProperty(newData, "Index", i++);
            SetProperty(newData, "CellPosx", cellPos.x);
            SetProperty(newData, "CellPosy", cellPos.y);
            SetProperty(newData, "CellPosz", cellPos.z);
            SetProperty(newData, "Tileid", map.id);

            Debug.Log(" GoogleSheetDataReadter >> WriteMap! ");

            qMapData.Enqueue(newData);
        }
    }

    private void SetProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        property.SetValue(obj, value);
    }


    private async Task asyncWriteDate()
    {
        if (isProcessing)
        {
            return;
        }

        isProcessing = true;

        while (qMapData.Count > 0)
        {
            if (!UnityPlayerWebRequest.Instance.reqProcessing)
            {
                var newData = qMapData.Dequeue();
                var newDataType = newData.GetType();

                //UnityGoogleSheet.Write<MapInformation.Data>(newDataType);

                var method = typeof(UnityGoogleSheet).GetMethod("Write", BindingFlags.Public | BindingFlags.Static);

                if (method != null)
                {
                    Debug.Log(" GoogleSheetDataReader >> GoogleSheet Add! ");
                    var genericMethod = method.MakeGenericMethod(newDataType);
                    genericMethod.Invoke(null, new object[] { newData, null });
                }
                else
                {
                    Debug.LogError(" GoogleSheetDataReader >> Write method not found!");
                }

            }
            await Task.Yield();
        }
        isProcessing = false;
    }

    /// <summary>
    /// GoogleDrive - Click Generate Button Data Load
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector3, MapData> ReadGenerateGoogleToMapData()
    {
        var mapDatas = new Dictionary<Vector3, MapData>();
        UnityGoogleSheet.Load<MapInformation.Data1>();

        MapInformation.Data1.Data1List.ForEach(x =>
        {
            int posX = (int)x.CellPosX;
            int posY = (int)x.CellPosY;
            int posZ = (int)x.CellPosZ;
            int id = x.TileID;

            Vector3 cellPos = new Vector3(posX, posY, posZ);

            if (id != 99)
            {
                GameObject target = Instantiate(items[id].tileObject, transform);
                MapData mapData = target.AddComponent<MapData>();
                mapData.id = id;
                mapData.cellPos = cellPos;
                mapData.transform.position = cellPos;

                mapDatas[cellPos] = mapData;
            }
        });

        return mapDatas;
    }

    

    /// <summary>
    /// GoogleDrive - RealTime Data Load
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector3, MapData> ReadRealTimeGoogleToMapData()
    {
        var mapDatas = new Dictionary<Vector3, MapData>();

        var method = typeof(UnityGoogleSheet).GetMethod("LoadFromGoogle", BindingFlags.Public | BindingFlags.Static);
        
        var genericMethod = method.MakeGenericMethod(typeof(int), GetSaveMapType());
        //UnityGoogleSheet.LoadFromGoogle<int, MapInformation.Data1>


        Action<IList, object> processData = (list, map) =>
        {
            foreach (var item in list)
            {
                int posX = (int)GetProperty(item, "CellPosx");
                int posY = (int)GetProperty(item, "CellPosy");
                int posZ = (int)GetProperty(item, "CellPosz");
                int id = (int)GetProperty(item, "Tileid");

                Vector3 cellPos = new Vector3(posX, posY, posZ);

                if (id != 99)
                {
                    GameObject target = Instantiate(items[id].tileObject, transform);
                    MapData mapData = target.AddComponent<MapData>();
                    mapData.id = id;
                    mapData.cellPos = cellPos;
                    mapData.transform.position = cellPos;

                    mapDatas[cellPos] = mapData;
                }
            }
        };

        return mapDatas;
    }
    private object GetProperty(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }
    private IList GetDataList(Type dataType)
    {
        var dataList = dataType.GetProperty("Data1List", BindingFlags.Public | BindingFlags.Static) ??
            dataType.GetProperty("Data2List", BindingFlags.Public | BindingFlags.Static) ??
            dataType.GetProperty("Data3List", BindingFlags.Public | BindingFlags.Static) ??
            dataType.GetProperty("Data4List", BindingFlags.Public | BindingFlags.Static);

        return dataList.GetValue(null) as IList;
    }
}
