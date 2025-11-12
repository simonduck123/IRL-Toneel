using UnityEngine;

[CreateAssetMenu(fileName = "OSCCommand", menuName = "OSC/Command")]
public class OSCCommand : ScriptableObject
{
    [Tooltip("OSC address (e.g. /distro/bull/idle)")]
    public string address;
}