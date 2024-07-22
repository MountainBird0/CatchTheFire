using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using UGS;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
class ObjectCheatEditor : NetworkBehaviour
{
    public GameObject waitObj;
    public CheatManager cheatmanager;


    public InputField Index;
    public InputField strValue;
    public InputField hp;
    public InputField hpRate;
    public InputField size;
    public InputField sizeUp;
    public InputField power;
    public InputField range;
    int selectnum = 0;

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

    public void ClickBtnSave()
    {
        int[] hps = ConvertTointList(hp.text).ToArray();
        int.TryParse(hpRate.text, out var HpRate);
        float.TryParse(size.text, out var Size);
        float.TryParse(sizeUp.text, out var SizeUp);
        float.TryParse(power.text, out var Power);

        switch (selectnum)
        {
            case 0:
                CheatManager.Instance.ReqCheatFire_Legacy(hps, HpRate, Size, SizeUp, Power);
                break;
            case 1:
                CheatManager.Instance.ReqCheatWater(hps, HpRate, Size, SizeUp, Power);
                break;
            case 2:
                CheatManager.Instance.ReqCheatCapsule(hps, HpRate, Size, SizeUp, Power);
                break;
            case 3:
                CheatManager.Instance.ReqCheatSandBags(hps, HpRate, Size, SizeUp, Power);
                break;
            case 4:
                CheatManager.Instance.ReqCheatIceHandCuffs(hps, HpRate, Size, SizeUp, Power);
                break;
        }


    }

    public void SelectData(String str)
    {
        DataManager dataManager = DataManager.Instance;
        AllObjectStats.ObjectData obj = dataManager.objectStats[str];
        Index.text = obj.Index.ToString();
        strValue.text = obj.StrValue;
        hp.text = ListToString(obj.Hp);
        hpRate.text = obj.HpRate.ToString();
        size.text = obj.Size.ToString();
        sizeUp.text = obj.SizeUp.ToString();
        power.text = obj.Power.ToString();
        range.text = obj.Range.ToString();
        selectnum = obj.Index;
    }

    public void SelectWrite()
    {

    }
}


