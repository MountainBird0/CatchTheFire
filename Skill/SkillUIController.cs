using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class SkillUIController : NetworkBehaviour
{
    public List<SkillSlot> skillSlots;

    public static SkillUIController instance;
    private void Awake()
    {
        instance = this;
    }
}
