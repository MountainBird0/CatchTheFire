using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour
{
    public List<GameObject> jobs;
   
    // temp
    public GameObject SelectJob(int jobNum)
    {
        GameObject ob = Instantiate(jobs[jobNum]);

        return ob;
    }
}
