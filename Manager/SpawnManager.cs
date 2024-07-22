using Unity.Netcode;
using UnityEngine;


public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void SpawnObject(GameObject prefab, GameObject tile)
    {
        Tile tileCheck = tile.GetComponent<Tile>();
        if (tileCheck.onTileObject != null)
            return;
        Vector3 pos = tile.transform.position;
        pos.y = 2;

        var ob = Instantiate(prefab, pos, Quaternion.identity);
        tileCheck.onTileObject = ob.GetComponent<SandBag>();
        NetworkObject Netobject = ob.GetComponent<NetworkObject>();
        Netobject.Spawn();
    }

    public void SpawnObject(GameObject prefab, Transform transform)
    {
     var ob = Instantiate(prefab, transform.position, Quaternion.identity);
     NetworkObject Netobject = ob.GetComponent<NetworkObject>();
     Netobject.Spawn();
    }
}
