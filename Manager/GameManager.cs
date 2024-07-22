using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkSingletonComponent<GameManager>
{
    public CheatController cheatController;
    public int selectedJobNum = 0;

    void Awake()
    {
        SetInstance();
    }
    
    public void Start()
    {
#if DEDICATED_SERVER
        UtilManager.InActiveCamera();
#endif
    }

    public void ChangeScene(int num)
    {
        // Change Asynchronous
        SceneManager.LoadScene(num);
    }

    public void SetBattle(Player player)
    {
        //var jobOb = BattleMapManager.Instance.characterSpawner.SelectJob(selectedJobNum);
        //jobOb.transform.SetParent(player.transform);
        player.transform.position = new Vector3(11, 2, 4);
    }

    public void SetSelectedJobNum(int num)
    {
        selectedJobNum = num;
    }

   
}
