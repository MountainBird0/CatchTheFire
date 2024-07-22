using UnityEngine;

public class SpeedUpEffect : BuffEffect
{
    private Character target;

    public SpeedUpEffect(float duration, Character target) : base(duration)
    {
        this.target = target;
    }

    public override void ApplyEffect()
    {
        target.controller.moveSpeed.Value = 2.0f;
    }

    public override void RemoveEffect()
    {
        target.controller.moveSpeed.Value = 1.5f;
    }
}
