using UnityEngine;

[ExecuteAlways]
public class LockRotationXZ : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 rot = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(0f, rot.y, 0f);
    }
}
