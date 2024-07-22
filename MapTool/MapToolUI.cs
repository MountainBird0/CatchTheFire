using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class MapToolUI : MonoBehaviour
{
    [SerializeField] protected float OffPos, OnPos = 0;
    [SerializeField] protected float tweenDuration;
    [SerializeField] protected Ease ease;

    [SerializeField] protected RectTransform gridSettingPanel;
    [SerializeField] protected Button gridRectButton;

    [SerializeField] protected Image buttonImage;
    [SerializeField] protected float buttonOffValue,buttonOnValue = 0;
    protected bool isShow = false;


    protected virtual void Awake()
    {
        InitPanel();
        gridRectButton.onClick.AddListener(() =>
        {
            isShow = !isShow;
            float newPos = isShow ? OnPos : OffPos;
            float newRotation = isShow ? buttonOnValue : buttonOffValue;
            ShowPanel(newPos);
            ChangeButtonImage(newRotation);
        });
    }

    protected virtual void InitPanel()
    {
        gridSettingPanel.transform.localPosition = new Vector3(OffPos, 0, 0);
    }

    protected virtual void ChangeButtonImage(float location)
    {
    }

    protected virtual void ShowPanel(float pos)
    {
        gridSettingPanel.DOAnchorPosX(pos, tweenDuration).SetEase(ease);
    }

}
