using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CheatManager : NetworkBehaviour
{
    public GameObject cheatCanvas = null;
    public List<Criminal> criminals = null;
    public FireFighter fireFighter = null;
    public Police police = null;
    public List<Fire> fires = null;
    public List<Water> waters = null;
    public List<ExtinguishingCapsule> capsules = null;
    public List<IceHandcuffs> iceHandcuffs = null;
    public List<SandBag> sandBags = null;

    public static CheatManager Instance;
    void Awake()
    {
        Instance = this;
    }

    public void ShowCheat()
    {
        cheatCanvas.SetActive(true);
        if (IsClient)
            CashingData();
    }

    public void HideCheat()
    {
        cheatCanvas.SetActive(false);
    }

    public void ReqCheatCriminal(int index, int jobid, float moveSpeed, float[] adjustedmovespeed, float size, float sizeup, float hp, float[] hprate, int[] flamegauge, float attachedflamegauge, float nomalflamegauge)
    {
        CheatCriminalServerRpc(index, jobid, moveSpeed, adjustedmovespeed, size, sizeup, hp, hprate, flamegauge, attachedflamegauge, nomalflamegauge);
    }
    public void ReqCheatFireFighter(int jobid, float moveSpeed, float inFire, float hp, int[] resistancePointChangeRate, float resistancePointChangeRateUp, float extinguishingPointChangeRate, float extinguishingPointChangeRateMax, float extinguisher, float extinguisherAddLate, float extinguisherTime)
    {
        CheatFireFigherServerRpc(jobid, moveSpeed, inFire, hp, resistancePointChangeRate, resistancePointChangeRateUp, extinguishingPointChangeRate, extinguishingPointChangeRateMax, extinguisher, extinguisherAddLate, extinguisherTime);
    }
    public void ReqCheatPolice(int jobid, float moveSpeed, float sandMoveSpeed)
    {
        CheatPoliceServerRpc(jobid, moveSpeed, sandMoveSpeed);
    }
    public void ReqCheatFire_Legacy(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        CreateFireServerRpc_LegacyServerRpc(Hp, HpRate, Size, SizeUp, Power);
    }
    public void ReqCheatFire(int[] maxHps, int startHp, int hpChangeRate, int maxLevel, int[] recoveryValue, float defaultSize, float sizeUpValue, int minClockLevel, int SpreadLevel)
    {
        CreateFireServerRpc(maxHps, startHp, hpChangeRate, maxLevel, recoveryValue, defaultSize, sizeUpValue, minClockLevel, SpreadLevel);
    }
    public void ReqCheatWater(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        CreateWaterServerRpc(Hp, HpRate, Size, SizeUp, Power);
    }
    public void ReqCheatCapsule(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        CreateCapsuleServerRpc(Hp, HpRate, Size, SizeUp, Power);
    }
    public void ReqCheatSandBags(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        CreateSandServerRpc(Hp, HpRate, Size, SizeUp, Power);
    }
    public void ReqCheatIceHandCuffs(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        CreateIceHandCuffServerRpc(Hp, HpRate, Size, SizeUp, Power);
    }

    #region  ServerRpc

    [ServerRpc(RequireOwnership = false)]
    public void CheatCriminalServerRpc(int index, int jobid, float movespeed, float[] adjustedmovespeed, float size, float sizeup, float hp, float[] hprate, int[] flamegauge, float attachedflamegauge, float nomalflamegauge)
    {
        Debug.Log("CheatManager >> CheatCriminalServerRpc");
        CashingData();
        if (criminals == null)
        {
            Debug.LogError("CheatManager >> CheatCriminalServerRpc : Can't Find criminal");
            return;
        }
        criminals[index].criminalstats.JobId = jobid;
        criminals[index].criminalstats.MoveSpeed = movespeed;
        criminals[index].criminalstats.AdjustedMoveSpeed = adjustedmovespeed.ToList();
        criminals[index].criminalstats.Size = size;
        criminals[index].criminalstats.SizeUp = sizeup;
        criminals[index].criminalstats.Hp = hp;
        criminals[index].criminalstats.HpRate = hprate.ToList();
        criminals[index].criminalstats.FlameGauge = flamegauge.ToList();
        criminals[index].criminalstats.AttachedFlameGauge = attachedflamegauge;
        criminals[index].criminalstats.NomalFlameGauge = nomalflamegauge;
        criminals[index].controller.moveSpeed.Value = movespeed;
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheatFireFigherServerRpc(int jobid, float moveSpeed, float inFire, float hp,
        int[] resistancePointChangeRate, float resistancePointChangeRateUp, float extinguishingPointChangeRate,
        float extinguishingPointChangeRateMax, float extinguisher, float extinguisherAddLate, float extinguisherTime)
    {
        Debug.Log("CheatManager >> CheatFireFigherServerRpc");
        CashingData();
        if (fireFighter == null)
        {
            Debug.LogError("CheatManager >> CheatFireFigherServerRpc : Can't Find FireFigher");
            return;
        }
        fireFighter.fireFighterstats.JobId = jobid;
        fireFighter.fireFighterstats.MoveSpeed = moveSpeed;
        fireFighter.fireFighterstats.InFire = inFire;
        fireFighter.fireFighterstats.Hp = hp;
        fireFighter.fireFighterstats.ResistancePointChangeRate = resistancePointChangeRate.ToList();
        fireFighter.fireFighterstats.ResistancePointChangeRateUp = resistancePointChangeRateUp;
        fireFighter.fireFighterstats.ExtinguishingPointChangeRate = extinguishingPointChangeRate;
        fireFighter.fireFighterstats.ExtinguishingPointChangeRateMax = extinguishingPointChangeRateMax;
        fireFighter.fireFighterstats.Extinguisher = extinguisher;
        fireFighter.fireFighterstats.ExtinguisherAddLate = extinguisherAddLate;
        fireFighter.fireFighterstats.ExtinguisherTime = extinguisherTime;
        fireFighter.controller.moveSpeed.Value = moveSpeed;
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheatPoliceServerRpc(int jobid, float moveSpeed, float sandMoveSpeed)
    {
        Debug.Log("CheatManager >> CheatPoliceServerRpc");
        CashingData();
        if (police == null)
        {
            Debug.LogError("CheatManager >> CheatPoliceServerRpc : Can't Find Police");
            return;
        }

        police.policeStats.JobId = jobid;
        police.policeStats.MoveSpeed = moveSpeed;
        police.policeStats.SandbagMoveSpeed = sandMoveSpeed;
        police.controller.moveSpeed.Value = moveSpeed;
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateFireServerRpc_LegacyServerRpc(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        Debug.Log("CheatManager >> CheatFireServerRpc");
        CashingData();

        foreach (Fire fire in fires)
        {
            fire.stats_Legacy.Hp = Hp.ToList();
            fire.stats_Legacy.HpRate = HpRate;
            fire.stats_Legacy.Size = Size;
            fire.stats_Legacy.SizeUp = SizeUp;
            fire.stats_Legacy.Power = Power;
        }

        AllObjectStats.ObjectData fireObjectStats = new();
        fireObjectStats.Hp = Hp.ToList();
        fireObjectStats.HpRate = HpRate;
        fireObjectStats.Size = Size;
        fireObjectStats.SizeUp = SizeUp;
        fireObjectStats.Power = Power;

        DataManager.Instance.objectStats["Fire"] = fireObjectStats;
    }
    [ServerRpc(RequireOwnership = false)]
    public void CreateFireServerRpc(int[] maxHps, int startHp, int hpChangeRate, int maxLevel, int[] recoveryValue, float defaultSize, float sizeUpValue, int minClockLevel, int SpreadLevel)
    {
        Debug.Log("CheatManager >> CheatFireServerRpc");
        CashingData();

        foreach (Fire fire in fires)
        {
            fire.stats.maxHps = maxHps.ToList();
            fire.stats.startHp = startHp;
            fire.stats.hpChangeRate = hpChangeRate;
            fire.stats.maxLevel = maxLevel;
            fire.stats.recoveryValue = recoveryValue.ToList();
            fire.stats.defaultSize = defaultSize;
            fire.stats.sizeUpValue = sizeUpValue;
            fire.stats.minClockLevel = minClockLevel;
            fire.stats.spreadLevel = SpreadLevel;
        }

        FireData stats = new();
        stats.maxHps = maxHps.ToList();
        stats.startHp = startHp;
        stats.hpChangeRate = hpChangeRate;
        stats.maxLevel = maxLevel;
        stats.recoveryValue = recoveryValue.ToList();
        stats.defaultSize = defaultSize;
        stats.sizeUpValue = sizeUpValue;
        stats.minClockLevel = minClockLevel;
        stats.spreadLevel = SpreadLevel;

        DataManager.Instance.fireStats = stats;
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateWaterServerRpc(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        Debug.Log("CheatManager >> CheatFireServerRpc");
        CashingData();

        foreach (Water water in waters)
        {
            water.waterStats.Hp = Hp.ToList();
            water.waterStats.HpRate = HpRate;
            water.waterStats.Size = Size;
            water.waterStats.SizeUp = SizeUp;
            water.waterStats.Power = Power;
        }

        AllObjectStats.ObjectData waterStats = new();
        waterStats.Hp = Hp.ToList();
        waterStats.HpRate = HpRate;
        waterStats.Size = Size;
        waterStats.SizeUp = SizeUp;
        waterStats.Power = Power;

        DataManager.Instance.objectStats["Water"] = waterStats;
    }
    [ServerRpc(RequireOwnership = false)]
    public void CreateCapsuleServerRpc(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        Debug.Log("CheatManager >> CheatFireServerRpc");
        CashingData();

        foreach (ExtinguishingCapsule capsule in capsules)
        {
            capsule.extinguishingCapsuleStats.Hp = Hp.ToList();
            capsule.extinguishingCapsuleStats.HpRate = HpRate;
            capsule.extinguishingCapsuleStats.Size = Size;
            capsule.extinguishingCapsuleStats.SizeUp = SizeUp;
            capsule.extinguishingCapsuleStats.Power = Power;
        }

        AllObjectStats.ObjectData capsuleStats = new();
        capsuleStats.Hp = Hp.ToList();
        capsuleStats.HpRate = HpRate;
        capsuleStats.Size = Size;
        capsuleStats.SizeUp = SizeUp;
        capsuleStats.Power = Power;

        DataManager.Instance.objectStats["Capulse"] = capsuleStats;
    }
    [ServerRpc(RequireOwnership = false)]
    public void CreateSandServerRpc(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        Debug.Log("CheatManager >> CheatFireServerRpc");
        CashingData();

        foreach (SandBag sandBag in sandBags)
        {
            sandBag.sandBagStats.Hp = Hp.ToList();
            sandBag.sandBagStats.HpRate = HpRate;
            sandBag.sandBagStats.Size = Size;
            sandBag.sandBagStats.SizeUp = SizeUp;
            sandBag.sandBagStats.Power = Power;
        }

        AllObjectStats.ObjectData sandStats = new();
        sandStats.Hp = Hp.ToList();
        sandStats.HpRate = HpRate;
        sandStats.Size = Size;
        sandStats.SizeUp = SizeUp;
        sandStats.Power = Power;

        DataManager.Instance.objectStats["Sandbox"] = sandStats;
    }
    [ServerRpc(RequireOwnership = false)]
    public void CreateIceHandCuffServerRpc(int[] Hp, int HpRate, float Size, float SizeUp, float Power)
    {
        Debug.Log("CheatManager >> CheatFireServerRpc");
        CashingData();

        foreach (IceHandcuffs iceHandcuff in iceHandcuffs)
        {
            iceHandcuff.iceHandcuffsStats.Hp = Hp.ToList();
            iceHandcuff.iceHandcuffsStats.HpRate = HpRate;
            iceHandcuff.iceHandcuffsStats.Size = Size;
            iceHandcuff.iceHandcuffsStats.SizeUp = SizeUp;
            iceHandcuff.iceHandcuffsStats.Power = Power;
        }

        AllObjectStats.ObjectData handCuffsStats = new();
        handCuffsStats.Hp = Hp.ToList();
        handCuffsStats.HpRate = HpRate;
        handCuffsStats.Size = Size;
        handCuffsStats.SizeUp = SizeUp;
        handCuffsStats.Power = Power;

        DataManager.Instance.objectStats["IceHandcuffs"] = handCuffsStats;
    }
    #endregion


    public void CashingData()
    {
        Debug.Log("CheatManager >> chashing");
        if (DataManager.Instance.criminals.Count > 0)
            criminals = DataManager.Instance.criminals;

        if (DataManager.Instance.fireFighter!= null)
        {
            fireFighter = DataManager.Instance.fireFighter;
        }
            
        if (DataManager.Instance.police != null)
            police = DataManager.Instance.police;

        if(DataManager.Instance.fires != null)
        {
            fires = DataManager.Instance.fires;
        }

        if (DataManager.Instance.fires != null)
        {
            waters = DataManager.Instance.waters;
        }

        if (DataManager.Instance.fires != null)
        {
            capsules = DataManager.Instance.capsules;
        }

        if (DataManager.Instance.fires != null)
        {
            sandBags = DataManager.Instance.sandBags;
        }

        if (DataManager.Instance.fires != null)
        {
            iceHandcuffs = DataManager.Instance.iceHandCuffs;
        }
    }
}
