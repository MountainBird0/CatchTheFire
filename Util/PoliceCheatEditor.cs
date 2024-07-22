using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
class PoliceCheatEditor : NetworkBehaviour
{
    private Police unit;
    public InputField JobId;
    public InputField MoveSpeed;
    public InputField SandbagMoveSpeed;

    private void OnEnable()
    {
        if (DataManager.Instance.police == null)
            return;
            unit =  DataManager.Instance.police;

        UpdateInputText();
    }
    
    public void UpdateInputText()
    {
        if (unit == null) return;
        JobId.text = unit.policeStats.JobId.ToString();
        MoveSpeed.text = unit.policeStats.MoveSpeed.ToString(); ;
        SandbagMoveSpeed.text = unit.policeStats.SandbagMoveSpeed.ToString();
    }

    public void ReqUpdateCheat()
    {     
        int.TryParse(JobId.text, out var jobId);
        float.TryParse(MoveSpeed.text, out var moveSpeed);
        float.TryParse(SandbagMoveSpeed.text, out var sandbagMoveSpeed);

        
        CheatManager.Instance.ReqCheatPolice(jobId,moveSpeed,sandbagMoveSpeed);
    }
    
}

