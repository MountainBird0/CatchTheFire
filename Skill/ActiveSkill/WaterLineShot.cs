using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaterLineShot : ActiveSkill
{
    public override void PerformSkill(params object[] obs)
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        ShotFireServerRpc(clientId, InputManager.instance.lastInputDir.normalized);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShotFireServerRpc(ulong clientId, Vector3 dir)
    {
        data.coolTime = 0.5f;
        data.aoeRange = 5;
        data.spreadSpeed = 0.5f;
        data.rangeType = ERangeType.LINE;

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
        int size = 2;

        Tile tile = tileQueue.Dequeue();
        tile.Extinguish();

        yield return new WaitForSeconds(data.spreadSpeed);

        while (tileQueue.Count > 0)
        {
            for (int i = 0; i < size; i++)
            {
                if (tileQueue.Count == 0)
                    yield break;

                tile = tileQueue.Dequeue();

                if (tile.isObstacle) continue;

                tile.Extinguish();
            }

            yield return new WaitForSeconds(data.spreadSpeed);
        }
    }
}
