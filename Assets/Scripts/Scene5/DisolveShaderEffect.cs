using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisolveShaderEffect : MonoBehaviour
{
    List<Material> materials = new List<Material>();

    void Start()
    {
        var renders = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            materials.AddRange(renders[i].materials);
        }
    }

    public void Dissolve(float duration = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(DissolveRoutine(duration));
    }

    private IEnumerator DissolveRoutine(float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            float value = t / duration; 
            SetValue(value);
            t += Time.deltaTime;
            yield return null;
        }

        SetValue(1f);
    }

    public void SetValue(float value)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].HasProperty("_Dissolve"))
                materials[i].SetFloat("_Dissolve", value);
        }
    }

}
