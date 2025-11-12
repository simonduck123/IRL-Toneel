using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    void Start()
    {
        Display.displays[1].Activate(3840, 2160, new RefreshRate() { numerator = 60, denominator = 1 });
    }
}
