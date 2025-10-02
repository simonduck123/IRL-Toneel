using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    void Start()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate(1440, 1050, new RefreshRate() { numerator = 60, denominator = 1 });
        }
    }
}
