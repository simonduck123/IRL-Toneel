using UnityEngine;
using UnityEngine.Events;
using extOSC;

public class OSCCommandHandler : MonoBehaviour
{
    public OSCManager manager;       // drag your OSCManager here
    public OSCCommand command;       // assign the command asset
    public UnityEvent<OSCMessage> onMessageReceived;

    void Start()
    {
        if (manager != null && command != null)
        {
            manager.RegisterHandler(command.address, (msg) => onMessageReceived.Invoke(msg));
        }
    }
}