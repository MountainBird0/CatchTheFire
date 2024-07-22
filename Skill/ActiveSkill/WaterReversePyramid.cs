using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class WaterReversePyramid : ActiveSkill
{
    public override void PerformSkill(params object[] obs)
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        ShotWaterServerRpc(clientId, InputManager.instance.lastInputDir.normalized);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShotWaterServerRpc(ulong clientId, Vector3 dir)
    {
        data.coolTime = 0.5f;
        data.aoeRange = 5;
        data.spreadSpeed = 0.5f;
        data.rangeType = ERangeType.CONE;

        Vector3 curPos = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform.position;

        Vector3Int tilePos = new Vector3Int((int)curPos.x, 0, (int)curPos.z);
        ExtinguishTiles(tilePos);
    }

    private void ExtinguishTiles(Vector3Int targetPos)
    {
        List<Tile> tiles = TileManager.Instance.SearchRange(data.rangeType, targetPos, data.aoeRange);

        Queue<Tile> tileQueue = new(tiles);
        StartCoroutine(ExtinguishTilesCoroutine(tileQueue));
    }
    private IEnumerator ExtinguishTilesCoroutine(Queue<Tile> tileQueue)
    {
        Vector3Int prePos = tileQueue.First().pos;

        while (tileQueue.Count > 0)
        {
            Tile tile = tileQueue.Dequeue();

            if (Vector3Int.Distance(prePos, tile.pos) > 1.0)
            {
                yield return new WaitForSeconds(data.spreadSpeed);
            }

            prePos = tile.pos;
            if (tile.isObstacle) continue;
            tile.Extinguish();
        }
    }
}
