using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private Light[] lights;
    
    public void TurnOnLights()
    {
        foreach (Light l in lights)
        {
            l.enabled = true;
        }
    }

    public void TurnOffLights()
    {
        foreach (Light l in lights)
        {
            l.enabled = false;
        }
    }
}
