using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MapToolUIPalette : MapToolUI
{
    [SerializeField] private Button potalButton;

    public CustomPaletteItem spawnPointObj; // Fix
    protected override void Awake()
    {
        base.Awake();

        potalButton.onClick.AddListener(() =>
        {
            TileMapEditor.Instance.drawTile = spawnPointObj;
        });
    }
    protected override void InitPanel()
    {
        gridSettingPanel.transform.localPosition = new Vector3(0, OffPos, 0);
    }
    protected override void ShowPanel(float pos)
    {
        gridSettingPanel.DOAnchorPosY(pos, tweenDuration).SetEase(ease);
    }
    protected override void ChangeButtonImage(float location)
    {
        buttonImage.transform.localRotation = Quaternion.Euler(0, location, 0);
    }
}
