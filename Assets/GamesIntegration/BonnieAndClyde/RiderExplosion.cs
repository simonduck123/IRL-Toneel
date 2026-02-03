using UnityEngine;
using UnityEngine.VFX;

public class RiderExplosion : MonoBehaviour
{
    private VisualEffect vfx;
    float time;

    void Awake()
    {
        vfx = GetComponent<VisualEffect>();
    }

    void Update()
    {
        time+=Time.deltaTime;

        if(time>2f)
            Destroy(gameObject);
    }
}