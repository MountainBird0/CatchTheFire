using Unity.Netcode;
using UnityEngine;

public class PowerUp : ActiveSkill
{
    public override void PerformSkill(params object[] obs)
    {
        PowerUpServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PowerUpServerRpc()
    {
        data.duration = 3f;

        foreach (var criminal in DataManager.Instance?.criminals)
        {
            if (criminal != null)
            {
                criminal.applyBuff(new NoCoolTimeEffect(data.duration, criminal, 0));
            }
        }
    }
}
