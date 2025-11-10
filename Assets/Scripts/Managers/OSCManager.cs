using UnityEngine;
using System.Collections.Generic;
using extOSC;

public class OSCManager : MonoBehaviour
{
    public static OSCManager Instance { get; private set; }

    [Header("OSC Settings")]
    public int ReceivePort = 7001;
    public OSCReceiver receiver;

    private Dictionary<string, System.Action<OSCMessage>> handlers = new();

    private void Start()
    {
        if (receiver != null)
        {
            // Bind a catch-all log callback
            receiver.Bind("*", OnAnyOSCMessage);
        }
    }


    private void OnAnyOSCMessage(OSCMessage message)
    {
        string values = "";

        for (int i = 0; i < message.Values.Count; i++)
        {
            values += message.Values[i].Value + (i < message.Values.Count - 1 ? ", " : "");
        }

        Debug.Log($"[OSC] {message.Address} | {values}");
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // prevent duplicates
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (receiver != null)
        {
            receiver.LocalPort = ReceivePort;
        }
        else
        {
            Debug.LogError("OSCManager: No OSCReceiver assigned!");
        }
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
