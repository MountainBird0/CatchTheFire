using UnityEngine;

[CreateAssetMenu(fileName = "FireStateColor", menuName = "Scriptable Object/ FireStateColor", order = 1)]
public class FireStateSO : ScriptableObject
{
    public Color inFireColor = Color.white;
    public Color outFireColor = Color.white;
}
