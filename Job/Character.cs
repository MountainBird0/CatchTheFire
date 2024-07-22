using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public CharacterStats stats;
    public NetworkVariable<ECharacterState> state = new(0);

    public PlayerController controller;
    public CharacterCondition condition;

    public List<Skill> activeSkills = new();
    public List<BuffEffect> buffList = new();

    private void Awake()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        Init();
    }

    private void Start()
    {
        
    }


    private void Init()
    {
        controller = GetComponentInParent<PlayerController>();

        DataManager.Instance.CashingCharacter(this);

        ulong clientId = NetworkManager.Singleton.LocalClientId;
        stats.jobIndex = (int)clientId;

        stats.characterName = CharacterStatus.Data.DataMap[stats.jobIndex].characterName;
        stats.moveSpeed = CharacterStatus.Data.DataMap[stats.jobIndex].moveSpeed;
        stats.fastMoveSpeed = CharacterStatus.Data.DataMap[stats.jobIndex].fastMoveSpeed;
        stats.hp = CharacterStatus.Data.DataMap[stats.jobIndex].hp;
        stats.faction = CharacterStatus.Data.DataMap[stats.jobIndex].faction;
        stats.skills = CharacterStatus.Data.DataMap[stats.jobIndex].skills;

        state.OnValueChanged += ChangeState;

        for (int i = 0; i < stats.skills.Count; i++)
        {
            var skill = SkillContainer.instance.skillList[stats.skills[i]];

            activeSkills.Add(skill);

            if(IsOwner)
            {
                var skillSlot = SkillUIController.instance.skillSlots[i];
            
                skillSlot.skill = skill;
                skillSlot.Init();
            }
        }

        controller.moveSpeed.Value = stats.moveSpeed;
    }

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

    private void ChangeState(ECharacterState pre, ECharacterState cur)
    {
        if ((pre & ECharacterState.IS_STUNNED) == 0
            && (cur & ECharacterState.IS_STUNNED) != 0)
        {
            StartCoroutine(Stun());
            if (IsServer)
            {
                StartCoroutine(Invincible());
            }
        }

        if ((pre & ECharacterState.IS_INVINCIBLE) == 0
            && (cur & ECharacterState.IS_INVINCIBLE) != 0)
        {
            if (IsServer)
            {
                StartCoroutine(Invincible());
            }
        }
    }

    private IEnumerator Stun()
    {
        controller.SetState(new StunState(controller));

        yield return new WaitForSeconds(1.0f);

        controller.SetState(new IdleState(controller));
        if (IsServer)
        {
            state.Value &= ~ECharacterState.IS_STUNNED;
        }
    }

    public IEnumerator Invincible()
    {
        state.Value |= ECharacterState.IS_INVINCIBLE;

        yield return new WaitForSeconds(6.0f);

        state.Value &= ~ECharacterState.IS_INVINCIBLE;
    }

    public void TakeHit(float damage)
    {
        condition.hp.curValue.Value += damage;
        if (condition.hp.curValue.Value == 5)
        {
            state.Value |= ECharacterState.IS_STUNNED;
        }
    }
}
