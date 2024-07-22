using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CustomPaletteItem : MonoBehaviour
{
    public Transform tileParent;
    public TMP_Text tileText;


    public int id;
    public GameObject tileObject;
    public string tileName;


    public void InitSetting(int _id, GameObject _tileObject, string _tileName)
    {
        this.id = _id;
        this.tileObject = _tileObject;
        this.tileName = _tileName;

        DrawItem();
    }

    public void DrawItem() //Vector2 slotSize, bool isSelected, CustomPaletteItem item
    {

        GameObject tile = Instantiate(tileObject, tileParent, false);
        tile.layer = 5;
        tileText.text = tileName;


        tile.transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Incremental)
                 .SetEase(Ease.Linear);

    }
}
