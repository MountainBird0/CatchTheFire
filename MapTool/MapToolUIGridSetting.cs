using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
public class MapToolUIGridSetting : MapToolUI
{
    public TMP_InputField gridSizeText;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        gridSizeText.onValueChanged.AddListener(SetGridSizeValue);
        GetInitGridSize();
    }

    private void GetInitGridSize()
    {
        gridSizeText.text = TileMapEditor.Instance.GridSize.ToString();
    }

    private void SetGridSizeValue(string text)
    {
        TileMapEditor.Instance.SetGridSize(text);
    }

    protected override void ChangeButtonImage(float location)
    {
        buttonImage.transform.localRotation = Quaternion.Euler(0, location, -90);
    }
}
