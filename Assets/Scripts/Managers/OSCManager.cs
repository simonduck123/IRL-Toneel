using UnityEngine;
using System.Collections.Generic;
using extOSC;

public class OSCManager : MonoBehaviour
{
    public int ReceivePort = 7001;
    public OSCReceiver receiver;

    [Tooltip("List of OSC commands to listen for")]
    public OSCCommand[] commands;

    private Dictionary<string, OSCCommand> _commandMap = new();

    void Awake()
    {
        receiver.LocalPort = ReceivePort;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrEmpty(cmd.address)) continue;

            // Cache
            _commandMap[cmd.address] = cmd;

            // Bind receiver
            receiver.Bind(cmd.address, (msg) => _commandMap[cmd.address].Invoke(msg));
        }
    }
}