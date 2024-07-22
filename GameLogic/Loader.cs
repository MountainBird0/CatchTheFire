using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private static EScene targetScene;

    public static void Load(EScene targetSene)
    {
        //Loader.targetScene = targetSene;
        SceneManager.LoadScene(targetSene.ToString());
    }

    public static void LoadNetwork(string targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }
}
