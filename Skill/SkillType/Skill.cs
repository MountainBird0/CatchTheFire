using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[SerializeField]
public struct SkillData
{
    public string imageIndex;

    public string skillName;
    public string explain;

    public int range;
    public int aoeRange;
    public float currentCoolTime;
    public float coolTime;
    public ETargetType targetType;

    public float damage;
    public float duration;
    public float projectileSpeed;
    public float spreadSpeed;

    public DirectionType directionType;
    public ERangeType rangeType;

    public int buttonIndex;
}


public abstract class Skill : NetworkBehaviour
{
    public Sprite image;
    public SkillData data;

    public abstract void PerformSkill(params object[] obs);
}
