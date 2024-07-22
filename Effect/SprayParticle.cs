using Unity.Netcode;
using UnityEngine;

public class SprayParticle : NetworkBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();   
    }

    [ClientRpc]
    public void PlayParticleClientRpc()
    {
        particle.Play();
    }

    [ClientRpc]
    public void StopParticleClientRpc()
    {
        particle.Stop();
    }
}
