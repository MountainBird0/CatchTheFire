using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    public Button skillButton;
    public TextMeshProUGUI coolTimeText;
    public Skill skill;

    // �̰� ��𿡼� ����?
    public void Init()
    {
        Debug.Log("����");
        // skill = skill // datamanager�� ���� �޾ƿ���
        skillButton.onClick.AddListener(() =>
        {
            skill.PerformSkill();
            StartCoroutine(SetCoolTime());
        });
    }

    public IEnumerator SetCoolTime()
    {
        skillButton.interactable = false;

        float currentCoolTime = skill.data.coolTime;

        while (currentCoolTime >= 0)
        {
            coolTimeText.text = currentCoolTime.ToString("F1");
            yield return new WaitForSeconds(0.1f);
            currentCoolTime -= 0.1f;
        }

        skillButton.interactable = true;
        coolTimeText.text = "button";
    }

}
