using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    void Start()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate(3200, 1200, new RefreshRate() { numerator = 60, denominator = 1 });
        }
    }
}
