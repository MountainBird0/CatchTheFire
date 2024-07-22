using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class WaterNormalAttack : ActiveSkill
{
    public override void PerformSkill(params object[] obs)
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        ThrowWaterServerRpc(clientId, InputManager.instance.lastInputDir.normalized);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowWaterServerRpc(ulong clientId, Vector3 dir)
    {
        data.coolTime = 0.5f;
        data.range = 3;
        data.aoeRange = 1;
        data.projectileSpeed = 0.5f;
        data.rangeType = ERangeType.CONSTANT;
        data.damage = 1f;

        Vector3 curPos = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform.position;

        Vector3 targetPos = curPos + (dir * data.range);
        Vector3 middlePos = curPos + (targetPos - curPos) / 2 + Vector3.up;

        var path = new Vector3[] { curPos, middlePos, targetPos };

        var netOb = NetworkObjectPoolLegacy.Singleton.Spawn("FireCapsule_Temp", curPos, Quaternion.identity);
        netOb.Spawn();

        Sequence sequence = DOTween.Sequence()
            .Append(netOb.transform.DOPath(path, data.projectileSpeed, PathType.CatmullRom).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    Destroy(netOb.gameObject);

                    Vector3Int tilePos = new Vector3Int((int)targetPos.x, 0, (int)targetPos.z);
                    ExtinguishEffect(data.rangeType, tilePos, data.aoeRange, data.damage);
                }); ;
    }

    private void ExtinguishEffect(ERangeType rangeType, Vector3Int targetPos, int range, float damage)
    {
        var tiles = TileManager.Instance.SearchRange(rangeType, targetPos, range);
        HashSet<Job> targets = new();

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].Extinguish();

            if (tiles[i].onTileplayers != null)
            {
                foreach (var target in tiles[i].onTileplayers)
                {
                    if (target.TryGetComponent<Job>(out var t))
                    {
                        targets.Add(t);
                    }
                }
            }

            foreach (var target in targets)
            {
                if (target.TryGetComponent<IStunDamageable>(out var t))
                {
                    t.TakeHit(damage);
                }
            }
        }
    }
}
