using UnityEngine;

public class ValuePrinter : MonoBehaviour
{
    // This will be called when you hook it up to OSCCommandHandler.onIntReceived
    public void PrintInt(int value)
    {
        Debug.Log($"Received INT: {value}");
    }

    // This will be called when you hook it up to OSCCommandHandler.onFloatReceived
    public void PrintFloat(float value)
    {
        Debug.Log($"Received FLOAT: {value}");
    }

    // This will be called when you hook it up to OSCCommandHandler.onStringReceived
    public void PrintString(string value)
    {
        Debug.Log($"Received STRING: {value}");
    }
}
