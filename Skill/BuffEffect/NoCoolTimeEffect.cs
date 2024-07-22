using UnityEngine;

public class NoCoolTimeEffect : BuffEffect
{
    private Job target;
    private int skillNum;

    public NoCoolTimeEffect(float duration, Job target, int skillNum) : base(duration)
    {
        this.target = target;
        this.skillNum = skillNum;
    }

    public override void ApplyEffect()
    {
        target.activeSkills[skillNum].data.coolTime = 0;
    }

    public override void RemoveEffect()
    {
        target.activeSkills[skillNum].data.coolTime = 0.25f;
    }
}
