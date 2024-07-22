using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Sirenix.Utilities;

public class CriminalCheatEditor : NetworkBehaviour
{
    private Criminal unit;
    public InputField Index;
    public InputField JobId;
    public InputField MoveSpeed;
    public InputField AdjustedMoveSpeed;
    public InputField Size;
    public InputField SizeUp;
    public InputField Hp;
    public InputField HpRate;
    public InputField FlameGauge;
    public InputField AttachedFlameGauge;
    public InputField NomalFlameGauge;

    private void OnEnable()
    {
        if (DataManager.Instance.criminals.Count <= 0)
            return;

        unit = DataManager.Instance.criminals[0];

        UpdateInputText();
    }

    // Update is called once per frame
    public void UpdateInputText()
    {
        if (unit == null) return;
        JobId.text = unit.criminalstats.JobId.ToString();
        MoveSpeed.text = unit.criminalstats.MoveSpeed.ToString();
        AdjustedMoveSpeed.text = ListToString(unit.criminalstats.AdjustedMoveSpeed);
        Size.text = unit.criminalstats.Size.ToString();
        SizeUp.text = unit.criminalstats.SizeUp.ToString();
        Hp.text = unit.criminalstats.Hp.ToString();
        HpRate.text = ListToString(unit.criminalstats.HpRate);
        FlameGauge.text = ListToString(unit.criminalstats.FlameGauge);
        AttachedFlameGauge.text = unit.criminalstats.AttachedFlameGauge.ToString();
        NomalFlameGauge.text = unit.criminalstats.NomalFlameGauge.ToString();
        Index.text = "0";
    }

    public void ReqUpdateCheat()
    {
        int.TryParse(Index.text, out var index);
        int.TryParse(JobId.text, out var jobId);
        float.TryParse(MoveSpeed.text, out var moveSpeed);
        List<float> adjustedMoveSpeedeList = ConvertToFloatList(AdjustedMoveSpeed.text);
        float[] adjustedMoveSpeed = adjustedMoveSpeedeList.ToArray();
        float.TryParse(Size.text, out var size);
        float.TryParse(SizeUp.text, out var sizeup);
        float.TryParse(Hp.text, out var hp);
        List<float> hpRateList = ConvertToFloatList(HpRate.text);
        float[] hpRate = hpRateList.ToArray();
        List<int> flameGaugeList = ConvertTointList(FlameGauge.text);
        int[] flameGauge = flameGaugeList.ToArray();
        float.TryParse(AttachedFlameGauge.text, out var attachedFlameGauge);
        float.TryParse(NomalFlameGauge.text, out var nomalFlameGauge);
        CheatManager.Instance.ReqCheatCriminal(index, jobId, moveSpeed, adjustedMoveSpeed, size, sizeup, hp, hpRate, flameGauge, attachedFlameGauge, nomalFlameGauge);
    }
    string ListToString(List<float> list)
    {
        return string.Join(", ", list);
    }
    string ListToString(List<int> list)
    {
        return string.Join(", ", list);
    }
    public List<float> ConvertToFloatList(string input)
    {
        return input.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => float.Parse(s))
                    .ToList();
    }

    public List<int> ConvertTointList(string input)
    {
        return input.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => int.Parse(s))
                    .ToList();
    }

    //public void UploadDataToGoogleSheet()
    //{
    //    var newData = new CharacterStatus.CriminalSt();

    //    newData.index = 0;
    //    newData.jobid = int.Parse(this.JobId.text);
    //    newData.moveSpeed = float.Parse(this.MoveSpeed.text);
    //    newData.adjustedMoveSpeed = ConvertToFloatList(this.AdjustedMoveSpeed.text);
    //    newData.size = float.Parse(this.Size.text);
    //    newData.sizeup = float.Parse(this.SizeUp.text);
    //    newData.hp = float.Parse(this.Hp.text);
    //    newData.hpRate = ConvertToFloatList(this.HpRate.text);
    //    newData.flameGauge = ConvertTointList(this.FlameGauge.text);
    //    newData.attachedflameGauge = float.Parse(this.AttachedFlameGauge.text);
    //    newData.nomalGauge = float.Parse(this.NomalFlameGauge.text);

    //    UnityGoogleSheet.Write<CharacterStatus.CriminalSt>(newData, (data) =>
    //    {
    //    });
    //    UploadDataToGoogleSheetServerRpc();
    //}
}
