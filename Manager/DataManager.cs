using CharacterStatus;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UGS;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DataManager : Singleton<DataManager>
{
    #region none
    public enum ItemOjbect { FIRE, WATER, CAPULSE, SANDBOX, ICEHANDCUFFS, END }
    public enum PlayerOjbect { CRIMINAL, FIREFIGHTER, POLICE, END }
    public AllObjectStats.CriminalData criminalStats = new AllObjectStats.CriminalData();
    public AllObjectStats.FireFighterData fireFighterStats = new AllObjectStats.FireFighterData();
    public AllObjectStats.PoliceData policeStats = new AllObjectStats.PoliceData();
    public Dictionary<String, AllObjectStats.ObjectData> objectStats = new Dictionary<string, AllObjectStats.ObjectData>();

    public FireData fireStats;

    public List<Criminal> criminals;
    public FireFighter fireFighter;
    public Police police;
    public List<Fire> fires;
    public List<Water> waters;
    public List<ExtinguishingCapsule> capsules;
    public List<SandBag> sandBags;
    public List<IceHandcuffs> iceHandCuffs;
    public List<Job> characterssss = new();
    #endregion

    public List<Character> characters;

    public void Awake()
    {
        DontDestroyOnLoad(this);

        CharacterStatus.Data.Load();
        SkillTable.Data.Load();

        CharacterStatus.CriminalSt.Load();
        CharacterStatus.FireFighterSt.Load();
        CharacterStatus.PoliceSt.Load();
        ObjectStatusTable.Data.Load();

        FireStatusTable.FiresStatus.Load();
    }
    async void Start()
    {
        SetDateCriminalSt();
        SetDateFireFighterSt();
        SetDatePoliceSt();
        SetDatatObjects();

        SetDataFireStatus();
    }

    private void SetDateCriminalSt()
    {
        foreach (var value in CharacterStatus.CriminalSt.CriminalStList)
        {
            criminalStats.Index = value.index;
            criminalStats.JobId = value.jobid;
            criminalStats.MoveSpeed = value.moveSpeed;
            criminalStats.AdjustedMoveSpeed = value.adjustedMoveSpeed;
            criminalStats.Size = value.size;
            criminalStats.SizeUp = value.sizeup;
            criminalStats.Hp = value.hp;
            criminalStats.HpRate = value.hpRate;
            criminalStats.FlameGauge = value.flameGauge;
            criminalStats.AttachedFlameGauge = value.attachedflameGauge;
            criminalStats.NomalFlameGauge = value.nomalGauge;
        }
    }
    private void SetDateFireFighterSt()
    {
        foreach (var value in CharacterStatus.FireFighterSt.FireFighterStList)
        {
            fireFighterStats.Index = value.index;
            fireFighterStats.JobId = value.jobid;
            fireFighterStats.MoveSpeed = value.moveSpeed;
            fireFighterStats.InFire = value.infire;
            fireFighterStats.Hp = value.hp;
            fireFighterStats.ResistancePointChangeRate = value.resistancePointChangeRate;
            fireFighterStats.ResistancePointChangeRateUp = value.resistancePointChangeRatedown;
            fireFighterStats.ExtinguishingPointChangeRate = value.extinguisingPointChangeRate;
            fireFighterStats.ExtinguishingPointChangeRateMax = value.extinguisingPointChangeRateMax;
            fireFighterStats.Extinguisher = value.extinguisher;
            fireFighterStats.ExtinguisherAddLate = value.extinguisheraddlate;
            fireFighterStats.ExtinguisherTime = value.extinguisherTime;
        }
    }
    private void SetDatePoliceSt()
    {
        foreach (var value in CharacterStatus.PoliceSt.PoliceStList)
        {
            policeStats.Index = value.index;
            policeStats.JobId = value.jobid;
            policeStats.MoveSpeed = value.moveSpeed;
            policeStats.SandbagMoveSpeed = value.sandbagMoveSpeed;
        }
    }
    private void SetDatatObjects()
    {
        foreach (var value in ObjectStatusTable.Data.DataList)
        {
            AllObjectStats.ObjectData obj = new AllObjectStats.ObjectData();
            switch (value.index)
            {
                case (int)ItemOjbect.FIRE:
                    obj.Index = 0;
                    obj.StrValue = value.strValue;
                    obj.Hp = value.hp;
                    obj.HpRate = value.hpRate;
                    obj.Size = value.size;
                    obj.SizeUp = value.sizeup;
                    obj.Power = value.power;
                    obj.Range = value.range;
                    objectStats.Add(value.strValue, obj);
                    break;
                case (int)ItemOjbect.WATER:
                    obj.Index = 1;
                    obj.StrValue = value.strValue;
                    obj.Hp = value.hp;
                    obj.HpRate = value.hpRate;
                    obj.Size = value.size;
                    obj.SizeUp = value.sizeup;
                    obj.Power = value.power;
                    obj.Range = value.range;
                    objectStats.Add(value.strValue, obj);
                    break;
                case (int)ItemOjbect.CAPULSE:
                    obj.Index = 2;
                    obj.StrValue = value.strValue;
                    obj.Hp = value.hp;
                    obj.HpRate = value.hpRate;
                    obj.Size = value.size;
                    obj.SizeUp = value.sizeup;
                    obj.Power = value.power;
                    obj.Range = value.range;
                    objectStats.Add(value.strValue, obj);
                    break;
                case (int)ItemOjbect.SANDBOX:
                    obj.Index = 3;
                    obj.StrValue = value.strValue;
                    obj.Hp = value.hp;
                    obj.HpRate = value.hpRate;
                    obj.Size = value.size;
                    obj.SizeUp = value.sizeup;
                    obj.Power = value.power;
                    obj.Range = value.range;
                    objectStats.Add(value.strValue, obj);
                    break;
                case (int)ItemOjbect.ICEHANDCUFFS:
                    obj.Index = 4;
                    obj.StrValue = value.strValue;
                    obj.Hp = value.hp;
                    obj.HpRate = value.hpRate;
                    obj.Size = value.size;
                    obj.SizeUp = value.sizeup;
                    obj.Power = value.power;
                    obj.Range = value.range;
                    objectStats.Add(value.strValue, obj);
                    break;
            }
        }
    }

    private void SetDataFireStatus()
    {
        var stat = FireStatusTable.FiresStatus.FiresStatusMap[0];

        fireStats.index = stat.index;
        fireStats.name = stat.name;
        fireStats.maxHps = stat.maxHps;
        fireStats.startHp = stat.startHp;
        fireStats.hpChangeRate = stat.hpChangeRate;
        fireStats.maxLevel = stat.maxLevel;
        fireStats.recoveryValue= stat.recoveryValue;
        fireStats.defaultSize = stat.defaultSize;
        fireStats.sizeUpValue= stat.sizeUpValue;
        fireStats.minClockLevel = stat.minClockLevel;
        fireStats.spreadLevel = stat.spreadLevel;
        fireStats.spreadType = stat.spreadType;
    }


    #region Cashing
    public void CashingCharacter(Character character)
    {
        characters.Add(character);
    }


    public void CashingData(Job playerJob)
    {
        if (playerJob == null)
            return;

        if ((int)PlayerOjbect.CRIMINAL == playerJob.jobindex)
        {
            criminals.Add(playerJob.gameObject.GetComponent<Criminal>());
        }
        else if ((int)PlayerOjbect.FIREFIGHTER == playerJob.jobindex)
        {
            fireFighter = playerJob.gameObject.GetComponent<FireFighter>();
        }
        else if ((int)PlayerOjbect.POLICE == playerJob.jobindex)
        {
            police = playerJob.gameObject.GetComponent<Police>();
        }

        Debug.Log($"{GetType()} - c:{criminals.Count}, f{fireFighter}, p : {police}");
    }

    public void CashingFire(Fire fire)
    {
        fires.Add(fire);
    }
    public void CashingWater(Water water)
    {
        waters.Add(water);
    }
    public void CashingCapsule(ExtinguishingCapsule capsule)
    {
        capsules.Add(capsule);
    }
    public void CashingSandBad(SandBag sand)
    {
        sandBags.Add(sand);
    }
    public void CashingIceHandCuffs(IceHandcuffs handcuffs)
    {
        iceHandCuffs.Add(handcuffs);
    }
    #endregion

}


