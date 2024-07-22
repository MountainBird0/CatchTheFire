using UnityEngine;

public abstract class BuffEffect
{
    public float duration;

    public BuffEffect(float duration)
    {
        this.duration = duration;
    }

    public abstract void ApplyEffect();
    public abstract void RemoveEffect();
}
