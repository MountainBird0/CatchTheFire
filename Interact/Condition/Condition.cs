using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Condition : NetworkBehaviour
{
    [SerializeField] private Image fillBar;
    [SerializeField] private TMP_Text currentValueText;

    // Todo : change to databundle
    // 
    public NetworkVariable<float> maxValue = new(0f);
    public NetworkVariable<float> startValue = new(0f);
    public NetworkVariable<float> curValue = new(0f);
    
    public NetworkVariable<float> inverseMaxValue = new(0f);

    private Transform cameraTransform;


    public override void OnNetworkSpawn()
    {
        //maxValue.Value = 100; // use for in update /0 nan error, need fix logic
        if (!IsServer) return;

        maxValue.OnValueChanged += GetPercentage;
        curValue.OnValueChanged += GetPercentage;

        // curValue.Value = startValue.Value;
    }

    private void Start()
    {
        cameraTransform = BattleMapManager.Instance.cinemachineCamera.transform;
        currentValueText.text = curValue.Value.ToString();
    }

    //private void LateUpdate()
    //{
    //    transform.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);

    //    Vector3 CameraRotation = cameraTransform.eulerAngles;
    //    if (CameraRotation.z != 0) { transform.Rotate(0f, 0f, -CameraRotation.z); }
    //}

    private void GetPercentage(float previousValue, float newValue)
    {
        currentValueText.text = curValue.Value.ToString();
        fillBar.fillAmount = curValue.Value / maxValue.Value;
    }

    public void SetCurValueWithChangeLate(float amount)
    {
        curValue.Value = Mathf.Clamp(curValue.Value + amount, 0f, maxValue.Value);
    }

    public void SetValue(float max, float start, float current)
    {
        Debug.Log("setvalue");
        maxValue.Value = max;
        startValue.Value = start;
        curValue.Value = current;
    }
}
