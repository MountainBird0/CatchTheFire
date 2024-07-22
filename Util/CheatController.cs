using System.Collections.Generic;
using UnityEngine;

public class CheatController : MonoBehaviour
{
    private Criminal criminal = null;
    private FireFighter fireFighter = null;
    private Police police = null;
    private List<Fire> fire = null;
    private List<Water> water = null;
    private List<ExtinguishingCapsule> capsule = null;
    private List<IceHandcuffs> iceHandcuffs = null;
    private List<SandBag> sandBag = null;
    
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Criminal"))
            criminal = GameObject.FindGameObjectWithTag("Criminal").GetComponent<Criminal>();
        if (GameObject.FindGameObjectWithTag("FireFighter"))
            fireFighter = GameObject.FindGameObjectWithTag("FireFighter").GetComponent<FireFighter>();
        if (GameObject.FindGameObjectWithTag("Police"))
            police = GameObject.FindGameObjectWithTag("Police").GetComponent<Police>();
        if (GameObject.FindGameObjectsWithTag("Fire").Length > 0)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Fire");
            foreach (GameObject gameObject in gameObjects)
            {
                fire.Add(gameObject.GetComponent<Fire>());
            }
        }

        if (GameObject.FindGameObjectsWithTag("Water").Length > 0)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Water");
        }
    }
}
