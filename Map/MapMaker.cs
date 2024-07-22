using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UGS;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MapMaker : NetworkBehaviour
{
    public static MapMaker Instance { get; set; }

    public Dictionary<Vector3, MapData> loadMapData;
    private List<Vector3> colliderDatas;
    public List<GameObject> items;
    public GameObject colliderPrefab;

#if UNITY_EDITOR
    string filePath = "Assets/Resources/mapData/";
#else
    string filePath = Application.streamingAssetsPath;
#endif
    public string fileName = "";
    string fileExtension = ".csv";

    private void Awake()
    {
        Instance = this;

        if (!IsServer) return;

        //List<List<Vector3>> datas = CraeteMapColliders(colliderDatas);

        //foreach (var data in datas)
        //{
        //    CreateCollider(data);
        //}
    }
    
    public async Task test()
    {
        //loadMapData = LoadCSVToMapData();
        loadMapData = await LoadGoogleSheetToMapDataAsync();
        //CreateMapBottomColliders(colliderDatas);
    }

    /// <summary>
    /// Load StreamAssets Folder - CSV File
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector3, MapData> LoadCSVToMapData()
    {
        string file = Path.Combine(filePath, fileName + fileExtension);

        var mapDatas = new Dictionary<Vector3, MapData>();
        colliderDatas = new List<Vector3>();

        try
        {
            string fileContent = File.ReadAllText(file);
            StringReader sr = new StringReader(fileContent);

            string line;
            bool isFirstLine = true;

            while ((line = sr.ReadLine()) != null)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                string[] values = Regex.Split(line, ",");

                int x = int.Parse(values[0]);
                int y = int.Parse(values[1]);
                int z = int.Parse(values[2]);
                int id = int.Parse(values[3]);

                Vector3 cellPos = new Vector3(x, y, z);

                if (id != 99)
                {
                    GameObject target = Instantiate(items[id], transform);

                    NetworkObject no = target.GetComponent<NetworkObject>();

                    no.Spawn();

                    MapData mapData = target.AddComponent<MapData>();
                    mapData.id = id;
                    mapData.cellPos = cellPos;
                    mapData.transform.position = cellPos;

                    mapDatas[cellPos] = mapData;
                    
                }
                else
                {
                    colliderDatas.Add(cellPos);

                    CreateCollider(cellPos);
                }

            }
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError($"File Not Found! : {e.Message}");
        }
        catch (IOException e)
        {
            Debug.LogError($"File Read Error : {e.Message}");
        }
        return mapDatas;
    }

    public async Task<Dictionary<Vector3, MapData>> LoadGoogleSheetToMapDataAsync()
    {
        var mapDatas = new Dictionary<Vector3, MapData>();
        colliderDatas = new List<Vector3>();

        var tcs = new TaskCompletionSource<bool>();

        UnityGoogleSheet.LoadFromGoogle<int, MapInformation.Data3>((list, map) =>
        {
            foreach(var x in list)
            {
                int posX = (int)x.CellPosX;
                int posY = (int)x.CellPosY;
                int posZ = (int)x.CellPosZ;
                int id = x.TileID;

                Vector3 cellPos = new Vector3(posX, posY, posZ);

                if (id != 99) // TODO : Change Enum Value
                {
                    GameObject target = Instantiate(items[id], transform);
                    NetworkObject no = target.GetComponent<NetworkObject>();

                    no.Spawn();
                    MapData mapData = target.AddComponent<MapData>();
                    mapData.id = id;
                    mapData.cellPos = cellPos;
                    mapData.transform.position = cellPos;

                    // if(id == 0)
                        mapDatas[cellPos] = mapData;

                }
                else if( id == 99)
                {
                    colliderDatas.Add(cellPos);

                    CreateCollider(cellPos);
                }
            }
            tcs.SetResult(true);
        }, true);


        await tcs.Task;
        return mapDatas;
    }


    private void CreateMapBottomColliders(List<Vector3> colDatas)
    {
        float minX = colDatas.Min(p => p.x);
        float maxX = colDatas.Max(p => p.x);
        float minZ = colDatas.Min(p => p.z);
        float maxZ = colDatas.Max(p => p.z);

        Vector3 pos = new Vector3((minX + maxX) / 2, 0.5f, (minZ + maxZ) / 2);
        Vector3 colSize = new Vector3(maxX - minX, 1, maxZ - minZ);

        GameObject ob = new GameObject($"Bottom_Collider");
        ob.transform.position = pos;
        ob.transform.localScale = colSize;
        ob.AddComponent<BoxCollider>();

    }


    private List<List<Vector3>> CraeteMapColliders(List<Vector3> colDatas)
    {
        //BFS
        List<List<Vector3>> groups = new();
        HashSet<Vector3> visited = new();


        foreach (var colData in colDatas)
        {
            if (!visited.Contains(colData))
            {
                List<Vector3> group = new();
                Queue<Vector3> queue = new();

                queue.Enqueue(colData);

                while (queue.Count > 0)
                {
                    Vector3 current = queue.Dequeue();
                    if (!visited.Contains(current))
                    {
                        visited.Add(current);
                        group.Add(current);

                        Vector3[] neighbors = new Vector3[]
                        {
                            current + Vector3.left,
                            current + Vector3.right,
                            current + Vector3.forward,
                            current + Vector3.back
                        };

                        foreach (var neighbor in neighbors)
                        {
                            if (colDatas.Contains(neighbor) && !visited.Contains(neighbor))
                            {
                                queue.Enqueue(neighbor);
                            }
                        }
                    }
                }

                groups.Add(group);
            }
        }

        return groups;
    }

    int i = 0;
    private void CreateCollider(List<Vector3> groups)
    {
        if (groups.Count == 0) return;

        float maxX = groups.Max(p => p.x);
        float minX = groups.Min(p => p.x);

        float maxZ = groups.Min(p => p.z);
        float minZ = groups.Max(p => p.z);

        Vector3 pos = new Vector3((minX + maxX) / 2, 0, (minZ + maxZ) / 2);
        Vector3 colSize = new Vector3(maxX - minX + 1, 10, maxZ - minZ + 1);

        GameObject ob = new GameObject($"Wall_Collider{i++}");
        ob.transform.position = pos;
        ob.transform.localScale = colSize;
        ob.AddComponent<BoxCollider>();

        Debug.Log($"{ob.name} - pos : {pos}, size : {colSize}");

    }

    private void CreateCollider(Vector3 pos)
    {
        if (colliderPrefab == null) return;

        //GameObject ob = new GameObject($"Wall_Collider{i++}");

        GameObject ob = Instantiate(colliderPrefab);
        ob.transform.position = pos;
        ob.transform.localScale = new (1f,10,1f);

        NetworkObject spawnNetObject = ob.GetComponent<NetworkObject>();
        spawnNetObject.Spawn();
    }

}


