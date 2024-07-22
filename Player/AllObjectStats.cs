using System;
using System.Collections.Generic;
using UnityEngine;

public class AllObjectStats : MonoBehaviour
{
    [Serializable]
    public struct CriminalData
    {
        public int Index { get; set; } // 캐릭터의 인덱스
        public int JobId { get; set; } // 캐릭터의 직업 ID

        public int faction; 

        // 속도 관련
        public float MoveSpeed { get; set; } // 이동 속도
        public List<float> AdjustedMoveSpeed { get; set; } // 조정된 이동 속도 리스트

        // 크기 관련
        public float Size { get; set; } // 캐릭터의 크기
        public float SizeUp { get; set; } // 캐릭터의 크기 증가율

        // 상태 관련
        public float Hp { get; set; } // 캐릭터의 체력
        public List<float> HpRate { get; set; } // 캐릭터의 체력

        // 화염 게이지 관련
        public List<int> FlameGauge { get; set; } // 화염 게이지 리스트
        public float AttachedFlameGauge { get; set; } // 부착된 화염 게이지 리스트
        public float NomalFlameGauge { get; set; } // 노말 화염 게이지 리스트
    }

    [Serializable]
    public struct FireFighterData
    {
        public int Index { get; set; } // 캐릭터의 인덱스
        public int JobId { get; set; } // 캐릭터의 직업 ID

        // 속도 관련
        public float MoveSpeed { get; set; } // 이동 속도

        // 상태 관련
        public float InFire { get; set; } // 화염 상태
        public float Hp { get; set; } // 캐릭터의 체력
        

        // 저항 관련
        public List<int> ResistancePointChangeRate { get; set; } // 저항 포인트 변화율 리스트
        public float ResistancePointChangeRateUp { get; set; } // 저항 포인트 감소율

        // 소화 관련
        public float ExtinguishingPointChangeRate { get; set; } // 소화 포인트 변화율
        public float ExtinguishingPointChangeRateMax { get; set; } // 최대 소화 포인트 변화율
        public float Extinguisher { get; set; } // 소화기
        public float ExtinguisherAddLate { get; set; } // 소화기 추가 속도
        public float ExtinguisherTime { get; set; } // 소화기 사용 시간
    }

    [Serializable]
    public struct PoliceData

    {
        public int Index { get; set; }       // 캐릭터의 인덱스
        public int JobId { get; set; }       // 캐릭터의 직업 ID
        public float MoveSpeed { get; set; } // 이동 속도
        public float SandbagMoveSpeed { get; set; }    // 화상 상태
    }

    [Serializable]
    public struct ObjectData
    {
        public int Index { get; set; }
        public string StrValue { get; set; }
        public List<int> Hp { get; set; }
        public int HpRate { get; set; }
        public float Size { get; set; }
        public float SizeUp { get; set; }
        public float Power { get; set; }
        public DirectionType Range { get; set; }
    }
}

[Serializable]
public struct CharacterStats
{
    public int jobIndex;
    public string characterName;
    public float moveSpeed;
    public float fastMoveSpeed;
    public int hp;
   
    public float faction;
    public List<int> skills;
}



[Serializable]
public struct FireData
{
    public int index;
    public string name;
    public List<int> maxHps;
    public int startHp;
    public int hpChangeRate;
    public int maxLevel;
    public List<int> recoveryValue;
    public float defaultSize;
    public float sizeUpValue;
    public int minClockLevel;
    public int spreadLevel;
    public DirectionType spreadType;
}

