using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public abstract class CheatEditor : NetworkBehaviour
{
    public void OnEnable()
    {
        UpdateInputText();
    }

    public abstract void UpdateInputText();
    public abstract void ClickBtnSave();

    protected string ListToString<T>(List<T> list)
    {
        return string.Join(", ", list);
    }

    protected List<T> ConvertToList<T>(string input)
    {
        return input.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => (T)Convert.ChangeType(s, typeof(T)))
                    .ToList();
    }

    protected T[] ConvertToArray<T>(string input)
    {
        return input.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => (T)Convert.ChangeType(s, typeof(T)))
                    .ToArray();
    }
}
