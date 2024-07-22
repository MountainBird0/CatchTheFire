using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Water : MonoBehaviour
{
    public UnityAction<GameObject> OnShootCancle;

    public AllObjectStats.ObjectData waterStats = new AllObjectStats.ObjectData();

    public void Awake()
    {
        DataManager.Instance.CashingWater(this);
    }

    private void OnEnable()
    {
        waterStats = DataManager.Instance.objectStats["Water"];
    }

    private void OnDisable()
    {
        OnShootCancle?.Invoke(this.gameObject);
    }
}
