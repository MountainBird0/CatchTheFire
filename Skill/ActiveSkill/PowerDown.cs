using Unity.Netcode;
using UnityEngine;

public class PowerDown : ActiveSkill
{
    public override void PerformSkill(params object[] obs)
    {
        if (obs[0] is int num)
            PowerDownServerRpc(num);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PowerDownServerRpc(int faction)
    {
        data.duration = 3f;
        data.targetType = ETargetType.ENEMY;
        int ownerFaction = faction;

        foreach (var character in DataManager.Instance?.characters)
        {
            if (character != null)
            {
                character.applyBuff(new RangeDownEffect(data.duration, character, 0));
            }
        }

        //data.duration = 3f;


        //var characters = DataManager.Instance.characters;

        //for (int i = 0; i < characters.Count; i++)
        //{
        //    switch (data.targetType)
        //    {
        //        case ETargetType.ENEMY:
        //            if (ownerFaction != characters[i].stats.faction)
        //            {
        //                characters[i].applyBuff(new RangeDownEffect(data.duration, characters[i], 0));
        //            }
        //            break;
        //    }
        //}
    }
}
