using Unity.Netcode;
using UnityEngine;

public class WaterSinkHole : ActiveSkill
{
    public override void PerformSkill(params object[] obs)
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        ThrowFireServerRpc(clientId, InputManager.instance.lastInputDir.normalized);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowFireServerRpc(ulong clientId, Vector3 dir)
    {
        data.coolTime = 0.5f;
        data.range = 4;
        data.aoeRange = 1;
        data.projectileSpeed = 0.5f;
        data.rangeType = ERangeType.CONSTANT;
        data.damage = 1f;

        Vector3 curPos = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform.position;

        Vector3Int tilePos = new Vector3Int((int)curPos.x, 0, (int)curPos.z);
        ExtinguishTiles(data.rangeType, tilePos, data.aoeRange, data.damage);
    }

    private void ExtinguishTiles(ERangeType rangeType, Vector3Int targetPos, int range, float damage)
    {
        var tiles = TileManager.Instance.SearchRange(rangeType, targetPos, range);

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].isObstacle) continue;

            tiles[i].Extinguish();
        }
    }
}
