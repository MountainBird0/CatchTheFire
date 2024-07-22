using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.Netcode;
using UnityEngine;


public class ExtinguishingCapsule : TileObject
{
    
    public AllObjectStats.ObjectData extinguishingCapsuleStats = new AllObjectStats.ObjectData();


    private ExtinguisingCapsuleInteractionMachine interactionMachine;

    public float extinguisingValue = 20f;
    public GameObject extinguishingFluid;
    public Police police;


    private void Awake()
    {
        interactionMachine = GetComponent<ExtinguisingCapsuleInteractionMachine>();
        interactionMachine.extinguishingCapsule = this;
    
        DataManager.Instance.CashingCapsule(this);
    
    }
    private void Start()
    {
        //StartCoroutine("DestroyCapsule");
        extinguishingCapsuleStats = DataManager.Instance.objectStats["Capulse"];
    }

    public void ExplosionCapsule()
    {
        List<GameObject> tiles = TileManager.Instance.ExploreNeighborsFromTile((int)transform.position.x, (int)transform.position.z, DirectionType.All);

        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].TryGetComponent<Tile>(out var tile))
            {
                if (tile.onTileObject != null)
                {
                    if (tile.onTileObject is Fire fire)
                    {
                        //fire.SetFirePoint(-10);
                        Destroy(fire.gameObject); 
                    }
                }
            }
             //Instantiate(extinguishingFluid, tiles[i].gameObject.transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }
    public void AddCapsule()
    {
        Destroy(this.gameObject);
    }

    public void SetPolice(Police po)
    {
        police = po;
    }

    //private IEnumerator DestroyCapsule()
    //{
    //    yield return new WaitForSeconds(extinguishingCapsuleStats.Hp[0]);
    //    ReserveDestroy();
    //}

    //public void ReserveDestroy()
    //{
    //    police.iExtinguishingCapsuleCount.Value--;
    //    Destroy(this.gameObject);
    //}
}
