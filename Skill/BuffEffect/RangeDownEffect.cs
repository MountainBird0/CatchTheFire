using JetBrains.Annotations;
using UnityEngine;

public class RangeDownEffect : BuffEffect
{
    private Character target;
    private int skillNum;

    public RangeDownEffect(float duration, Character target, int skillNum) : base(duration)
    {
        this.target = target;
        this.skillNum = skillNum;
    }


    public override void ApplyEffect()
    {
        target.activeSkills[skillNum].data.aoeRange = 0;
    }

    public override void RemoveEffect()
    {
        target.activeSkills[skillNum].data.aoeRange = 1;
    }
}
