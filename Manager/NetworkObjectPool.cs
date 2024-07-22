using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

using Unity.Netcode;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[Serializable]
class Poolable
{
    [Required] public string Tag;
    
    [Required] [AssetSelector(Paths = "Assets/Prefabs")]
    public NetworkObject Prefab;
    
    public int Count;
}

// Odin Editor가 모든 하위 클래스에 대해 적용되도록 함
#if UNITY_EDITOR
[CustomEditor(typeof(NetworkBehaviour), true)]
public class OdinNetworkBehaviourEditor : OdinEditor{}
#endif

public class NetworkObjectPool : NetworkBehaviour
{
    public static NetworkObjectPool Instance { get; private set; }
    
    [SerializeField] private List<Poolable> _pools;
    
    private Dictionary<string, Queue<NetworkObject>> _poolMap = new();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        foreach (var pool in _pools)
        {
            var queue = new Queue<NetworkObject>();
            
            var parent = new GameObject();
            parent.name = pool.Tag;
            parent.transform.SetParent(transform);
            
            for (var i = 0; i < pool.Count; i++)
            {
                var networkObject = Instantiate(pool.Prefab, parent.transform);
                networkObject.gameObject.SetActive(false);
                queue.Enqueue(networkObject);
            }
            
            _poolMap.Add(pool.Tag, queue);

            NetworkManager.Singleton.PrefabHandler.AddHandler(pool.Prefab, new PooledPrefabInstanceHandler(this, pool.Tag));
        }
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
        foreach (var pool in _pools)
        {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(pool.Prefab);
            Destroy(transform.Find(pool.Tag).gameObject);
        }
        
        _poolMap.Clear();
    }
    
    public NetworkObject GetFromPool(string key, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        NetworkObject networkObject;
        
        if (_poolMap[key].Count == 0)
        {
            networkObject = Instantiate(_pools.Find(x => x.Tag == key).Prefab, transform.Find(key));
            networkObject.gameObject.SetActive(false);
        }
        else networkObject = _poolMap[key].Dequeue();
        if (networkObject == null) return null;
        
        networkObject.transform.position = position;
        networkObject.transform.rotation = rotation;
        
        if (parent != null) networkObject.transform.SetParent(parent);
        
        networkObject.gameObject.SetActive(true);
        return networkObject;
    }

    public void ReturnToPool(string key, NetworkObject networkObject)
    {
        var mapCount = _poolMap[key].Count;
        var poolCount = _pools.Find(x => x.Tag == key).Count;

        if (mapCount >= poolCount)
        {
            Destroy(networkObject.gameObject);
            return;
        }

        // networkObject.transform.SetParent(transform.Find(key));
        networkObject.Despawn();
        // networkObject.gameObject.SetActive(false);
        _poolMap[key].Enqueue(networkObject);
    }
}

class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    private NetworkObjectPool _pool;
    private string _tag;

    public PooledPrefabInstanceHandler(NetworkObjectPool pool, string tag)
    {
        _pool = pool;
        _tag = tag;
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        return _pool.GetFromPool(_tag, position, rotation);
    }

    public void Destroy(NetworkObject networkObject)
    {
        _pool.ReturnToPool(_tag, networkObject);
    }
}
