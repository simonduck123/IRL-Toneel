using UnityEngine;
using extOSC;

public class OSCReceiverTest : MonoBehaviour
{
    public int port = 7001;
    private OSCReceiver receiver;

    void Start()
    {
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = port;

        receiver.Bind("/test/1", OnTestMessage);

        Debug.Log($"OSC Receiver listening on port {port}");
    }

    void OnTestMessage(OSCMessage message)
    {
        Debug.Log($"✅ Received OSC: {message.Address}");
    }
}