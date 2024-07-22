using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Job : NetworkBehaviour
{
    public int jobindex;

    public CharacterStats stats;

    // public PlayerController controller;

    public List<ActiveSkill> activeSkills = new();
    public List<BuffEffect> buffList = new();

    public void applyBuff(BuffEffect effect)
    {
        effect.ApplyEffect();
        buffList.Add(effect);
        StartCoroutine(RemoveEffectAfterDuration(effect));
    }

    private IEnumerator RemoveEffectAfterDuration(BuffEffect effect)
    {
        yield return new WaitForSeconds(effect.duration);
        effect.RemoveEffect();
        buffList.Remove(effect);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeSkills[0].PerformSkill();
        }
    }

}
