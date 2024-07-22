using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillContainer : NetworkBehaviour
{
    public static SkillContainer instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Skill> skillList;
}
