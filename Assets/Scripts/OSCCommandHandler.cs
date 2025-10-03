using UnityEngine;
using extOSC;

public class OSCCommandHandler : MonoBehaviour
{
    public OSCCommand command;       // assign the command asset

    [Header("OSC Events")]
    public OSCMessageEvent onMessageReceived;
    public FloatEvent onFloatReceived;
    public IntEvent onIntReceived;
    public StringEvent onStringReceived;

    void Start()
    {
        if (OSCManager.Instance != null && command != null)
        {
            OSCManager.Instance.RegisterHandler(command.address, OnOSCMessage);
        }
    }

    void OnOSCMessage(OSCMessage msg)
    {
        // Always pass raw OSC message
        onMessageReceived?.Invoke(msg);

        // If message contains values, try to parse them
        if (msg.Values.Count > 0)
        {
            var value = msg.Values[0];

            if (value.Type == extOSC.OSCValueType.Float)
                onFloatReceived?.Invoke(value.FloatValue);

            else if (value.Type == extOSC.OSCValueType.Int)
                onIntReceived?.Invoke(value.IntValue);

            else if (value.Type == extOSC.OSCValueType.String)
                onStringReceived?.Invoke(value.StringValue);
        }
    }
}
