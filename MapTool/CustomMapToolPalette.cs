using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class CustomMapToolPalette : MonoBehaviour
{
    public CustomPaletteItem itemTemp;
    public List<GameObject> tileItems;


    private List<CustomPaletteItem> items = new();
    public Transform scrollContent;

    private void Start()
    {
        DrawItems();
        CSVDataReader.Instance.SetItemPaletteItem(items);
        GoogleSheetDataReader.Instance.SetItemPaletteItem(items);
    }

    private void DrawItems()
    {
        for(int i = 0; i < tileItems.Count; i++)
        {
            GameObject tileItem = tileItems[i];
            CustomPaletteItem newItem = Instantiate(itemTemp, scrollContent);
            newItem.InitSetting(i, tileItem, tileItem.name);

            Button itemBtn = newItem.GetComponent<Button>();

            if (itemBtn != null)
                itemBtn.onClick.AddListener(() => OnClickItem(newItem));


            items.Add(newItem);
        }
    }
    private void OnClickItem(CustomPaletteItem item)
    {
        TileMapEditor.Instance.drawTile = item;
    }
}
