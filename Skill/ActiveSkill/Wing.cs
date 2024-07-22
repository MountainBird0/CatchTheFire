using Unity.Netcode;
using UnityEngine;

public class Wing : ActiveSkill
{
    public override void PerformSkill(params object[] obs)
    {
        WingServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void WingServerRpc()
    {
        data.duration = 3f;

        foreach (var criminal in DataManager.Instance?.characters)
        {
            if (criminal != null)
            {
                criminal.applyBuff(new SpeedUpEffect(data.duration, criminal));
            }
        }
    }
}
