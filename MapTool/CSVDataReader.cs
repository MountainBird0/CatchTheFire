using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class CSVDataReader : SingletonComponent<CSVDataReader>
{
    public event Action<string> onSaveResult;
    public string fileName { get; set; }
    string fileExtension = ".csv";
#if UNITY_EDITOR
    string filePath = "Assets/Resources/mapData/";
#else
    string filePath = Application.streamingAssetsPath;
#endif



    private List<CustomPaletteItem> items;

    public void SetItemPaletteItem(List<CustomPaletteItem> _items)
    {
        this.items = _items;
    }

    public void WriteMapDataToCSV(Dictionary<Vector3, MapData> mapDatas)
    {
        //string file = filePath + fileName + fileExtension;
        string file = Path.Combine(filePath, fileName + fileExtension);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("CellPosX, CellPosY, CellPosZ, TileID");

        foreach(var mapdata in mapDatas)
        {
            Vector3 cellPos = mapdata.Key;
            MapData mapData = mapdata.Value;
            sb.AppendLine($"{cellPos.x},{cellPos.y},{cellPos.z},{mapData.id}");
        }

        try
        {
            SetSaveResult("Save Succeed!");
            File.WriteAllText(file, sb.ToString());
        }
        catch(IOException e)
        {
            SetSaveResult("@ Save Error @");
            Debug.LogError($"File Write Error : {e.Message}");
        }
    }

    public Dictionary<Vector3, MapData> ReadCSVToMapData() //string filePath,
    {
        //string file = filePath + fileName + fileExtension;
        string file = Path.Combine(filePath, fileName + fileExtension);

        var mapDatas = new Dictionary<Vector3, MapData>();


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

                if(id != 99)
                {
                    GameObject target = Instantiate(items[id].tileObject, transform);
                    MapData mapData = target.AddComponent<MapData>();
                    mapData.id = id;
                    mapData.cellPos = cellPos;
                    mapData.transform.position = cellPos;

                    mapDatas[cellPos] = mapData;
                }
            }
            SetSaveResult("Load Succeed!");
        }
        catch (FileNotFoundException e)
        {
            SetSaveResult("@ File Not Found @");
            Debug.LogError($"File Not Found! : {e.Message}");
        }
        catch (IOException e) 
        {
            SetSaveResult("@ Load Error @");
            Debug.LogError($"File Read Error : {e.Message}");
        }
        
        
        return mapDatas;
    }

    private void SetSaveResult(string resultTxt)
    {
        onSaveResult?.Invoke(resultTxt);
    }
}
