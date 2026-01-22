using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private Light[] lights;
    
    public void DoLights()
    {
        foreach (Light l in lights)
        {
            l.enabled = !l.enabled;
        }
    }
}
