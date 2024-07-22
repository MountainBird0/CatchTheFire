using UnityEngine;

public class CoolTimeUpEffect : BuffEffect
{
    private Character target;
    private int skillNum;

    public CoolTimeUpEffect(float duration, Character target, int skillNum) : base(duration)
    {
        this.target = target;
        this.skillNum = skillNum;
    }

    public override void ApplyEffect()
    {
        for(int i = 1; i < target.activeSkills.Count; i++)
        {
            target.activeSkills[i].data.currentCoolTime += 5; 
        }
    }

    public override void RemoveEffect()
    {
        //target.activeSkills[skillNum].data.currentCoolTime = 0.25f;
    }
}
