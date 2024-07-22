using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public partial class FireFighterCheatEditor : NetworkBehaviour
{
    private FireFighter unit;
    public InputField JobId;
    public InputField MoveSpeed;
    public InputField InFire;
    public InputField Hp;
    public InputField ResistancePointChangeRate;
    public InputField ResistancePointChangeRateUp;
    public InputField ExtinguishingPointChangeRate;
    public InputField ExtinguishingPointChangeRateMax;
    public InputField Extinguisher;
    public InputField ExtinguisherAddLate;
    public InputField ExtinguisherTime;
    private void OnEnable()
    {
        if (DataManager.Instance.fireFighter == null)
            return;
        unit = DataManager.Instance.fireFighter;

        UpdateInputText();


    }
    public void UpdateInputText()
    {
        if (unit == null) return;
        JobId.text = unit.fireFighterstats.JobId.ToString();
        MoveSpeed.text = unit.fireFighterstats.MoveSpeed.ToString(); ;
        InFire.text = unit.fireFighterstats.InFire.ToString();
        Hp.text = unit.fireFighterstats.Hp.ToString(); ;
        ResistancePointChangeRate.text = ListToString(unit.fireFighterstats.ResistancePointChangeRate); ;
        ResistancePointChangeRateUp.text = unit.fireFighterstats.ResistancePointChangeRateUp.ToString();
        ExtinguishingPointChangeRate.text = unit.fireFighterstats.ExtinguishingPointChangeRate.ToString();
        ExtinguishingPointChangeRateMax.text = unit.fireFighterstats.ExtinguishingPointChangeRateMax.ToString();
        Extinguisher.text = unit.fireFighterstats.Extinguisher.ToString();
        ExtinguisherAddLate.text = unit.fireFighterstats.ExtinguisherAddLate.ToString();
        ExtinguisherTime.text = unit.fireFighterstats.ExtinguisherTime.ToString();
    }

    public void ReqUpdateCheat()
    {
        int.TryParse(JobId.text, out var jobId);
        float.TryParse(MoveSpeed.text, out var moveSpeed);
        float.TryParse(InFire.text, out var infire);
        float.TryParse(Hp.text, out var hp);
        List<int> resistancePointChangeRateList = ConvertTointList(ResistancePointChangeRate.text);
        int[] resistancePointChangeRate = resistancePointChangeRateList.ToArray();
        float.TryParse(ResistancePointChangeRateUp.text, out var resistancePointChangeRateUp);
        float.TryParse(ExtinguishingPointChangeRate.text, out var extinguishingPointChangeRate);
        float.TryParse(ExtinguishingPointChangeRateMax.text, out var extinguishingPointChangeRateMax);
        float.TryParse(Extinguisher.text, out var extinguisher);
        float.TryParse(ExtinguisherAddLate.text, out var extinguisherAddLate);
        float.TryParse(ExtinguisherTime.text, out var extinguisherTime);
        CheatManager.Instance.ReqCheatFireFighter(jobId, moveSpeed, infire, hp, resistancePointChangeRate, resistancePointChangeRateUp
            , extinguishingPointChangeRate, extinguishingPointChangeRateMax, extinguisher, extinguisherAddLate, extinguisherTime);
    }
    // Update is called once per frame

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
    //    var newData = new CharacterStatus.FireFighterSt();

    //    newData.index = 0;
    //    newData.jobid = int.Parse(this.JobId.text);
    //    newData.moveSpeed = float.Parse(this.MoveSpeed.text);
    //    newData.infire = float.Parse(this.InFire.text);
    //    newData.hp = float.Parse(this.Hp.text);
    //    newData.resistancePointChangeRate = ConvertTointList(ResistancePointChangeRate.text);
    //    newData.resistancePointChangeRatedown = float.Parse(this.ResistancePointChangeRateUp.text);
    //    newData.extinguisingPointChangeRate = float.Parse(this.ExtinguishingPointChangeRate.text);
    //    newData.extinguisingPointChangeRateMax = float.Parse(this.ExtinguishingPointChangeRateMax.text);
    //    newData.extinguisher = float.Parse(this.Extinguisher.text);
    //    newData.extinguisheraddlate = float.Parse(this.ExtinguisherAddLate.text);
    //    newData.extinguisherTime = float.Parse(this.ExtinguisherTime.text);

    //    UnityGoogleSheet.Write<CharacterStatus.FireFighterSt>(newData, (data) =>
    //    {

    //        //});

    //    }
}
