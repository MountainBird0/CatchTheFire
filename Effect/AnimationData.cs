using UnityEngine;

[CreateAssetMenu(fileName = "ChracterAnimation", menuName = "Scriptable Object/ChracterAnimation", order = int.MaxValue)]
public class AnimationData : ScriptableObject
{
    public string animationState;
    public Sprite[] sprites;
    public float frame;
    public bool loop;
}
