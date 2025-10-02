using UnityEngine;
using UnityEngine.Events;
using extOSC;

[CreateAssetMenu(fileName = "OSCCommand", menuName = "OSC/Command")]
public class OSCCommand : ScriptableObject
{
    [Tooltip("OSC address (e.g. /distro/bull/idle)")]
    public string address;

    [Tooltip("What happens when this OSC command is received")]
    public UnityEvent<OSCMessage> onMessageReceived;

    public void Invoke(OSCMessage message)
    {
        onMessageReceived?.Invoke(message);
    }
}