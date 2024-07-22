using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireCheatEditor : CheatEditor
{
    [Header("InputField")]
    public TMP_InputField MaxHps;
    public TMP_InputField StartHp;
    public TMP_InputField HpChangeRate;
    public TMP_InputField MaxLevel;
    public TMP_InputField RecoveryValue;
    public TMP_InputField DefaultSize;
    public TMP_InputField SizeUpValue;
    public TMP_InputField MinClockLevel;
    public TMP_InputField SpreadLevel;
    //public TMP_InputField SpreadType;

    public override void UpdateInputText()
    {
        if (DataManager.Instance.fires.Count == 0) return;
        var stats = DataManager.Instance.fires[0].stats;

        MaxHps.text = ListToString(stats.maxHps);
        StartHp.text = stats.startHp.ToString();
        HpChangeRate.text = stats.hpChangeRate.ToString();
        MaxLevel.text = stats.maxLevel.ToString();
        RecoveryValue.text = ListToString(stats.recoveryValue);
        DefaultSize.text = stats.defaultSize.ToString();
        SizeUpValue.text = stats.sizeUpValue.ToString();
        MinClockLevel.text = stats.minClockLevel.ToString();
        SpreadLevel.text = stats.spreadLevel.ToString();
        //SpreadType.text = stats.spreadType.ToString();
    }

    public override void ClickBtnSave()
    {
        int[] maxHps = ConvertToArray<int>(MaxHps.text);
        int.TryParse(StartHp.text, out var startHp);
        int.TryParse(HpChangeRate.text, out var hpChangeRate);
        int.TryParse(MaxLevel.text, out var maxLevel);
        int[] recoveryValue = ConvertToArray<int>(RecoveryValue.text);
        float.TryParse(DefaultSize.text, out var defaultSize);
        float.TryParse(SizeUpValue.text, out var sizeUpValue);
        int.TryParse(MinClockLevel.text, out var minClockLevel);
        int.TryParse(SpreadLevel.text, out var spreadLevel);

        CheatManager.Instance.ReqCheatFire(maxHps, startHp, hpChangeRate, maxLevel, recoveryValue, defaultSize, sizeUpValue, minClockLevel, spreadLevel);
    }

}
