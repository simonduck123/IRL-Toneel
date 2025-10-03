using UnityEngine;
using System.Collections.Generic;
using extOSC;

public class OSCManager : MonoBehaviour
{
    public int ReceivePort = 7001;
    public OSCReceiver receiver;

    private Dictionary<string, System.Action<OSCMessage>> handlers = new();

    void Awake()
    {
        receiver.LocalPort = ReceivePort;
    }

    public void RegisterHandler(string address, System.Action<OSCMessage> handler)
    {
        if (!handlers.ContainsKey(address))
        {
            handlers[address] = handler;
            receiver.Bind(address, (msg) => handlers[address](msg));
        }
        else
        {
            handlers[address] += handler; // allow multiple listeners
        }
    }
}