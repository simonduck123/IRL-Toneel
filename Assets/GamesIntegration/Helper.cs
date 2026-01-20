using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static Vector2 EvaluateBezierSimple(Vector2 A, Vector2 B, Vector3 C, float p)
    {
        Vector2 AB = Vector2.Lerp(A, B, p);
        Vector2 BC = Vector2.Lerp(B, C, p);
        Vector2 AC = Vector2.Lerp(AB, BC, p);

        return AC;
    }

    static float ApproxBezierLength(Vector2 A, Vector2 B, Vector2 C, int samples = 50)
    {
        float length = 0f;
        Vector2 prev = A;

        for (int i = 1; i <= samples; i++)
        {
            float t = i / (float)samples;

            Vector2 a = Vector2.Lerp(A, B, t);
            Vector2 b = Vector2.Lerp(B, C, t);
            Vector2 p = Vector2.Lerp(a, b, t);

            length += Vector2.Distance(prev, p);
            prev = p;
        }

        return length;
    }

    public static Vector2 EvaluateBezier(
        Vector2 A, Vector2 B, Vector2 C,
        float p,             // progress 0..1
        float startCurve,    // D = Lerp(A,B,startCurve)
        float endCurve,      // E = Lerp(B,C,endCurve)
        float m1 = 1f,       // weight for A->D
        float m2 = 1f,       // weight for D->E
        float m3 = 1f       // weight for E->C
    )
    {

        Vector2 D = Vector2.Lerp(A, B, startCurve);
        Vector2 E = Vector2.Lerp(B, C, endCurve);

        float distAD = Vector2.Distance(A,D)*m1;
        float distCurveED = ApproxBezierLength(E,B,D)*m2;
        float distEC = Vector2.Distance(E,C)*m3;
        
      
        float sumDistances = distAD+distEC+distCurveED;
        float k1 = distAD/sumDistances;
        float k2 = distCurveED/sumDistances;
       
        if (p <= k1)
            return Vector2.Lerp(A, D, p / k1);
        else if (p <= k1+k2)
            return EvaluateBezierSimple(D,B,E,Remap(p,k1,k1+k2,0f,1f));
        else
            return Vector2.Lerp(E, C,Remap(p,k1+k2,1f,0f,1f));
    }


    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float RemapClamp(float value, float from1, float to1, float from2, float to2)
    {
        return Mathf.Clamp((value - from1) / (to1 - from1) * (to2 - from2) + from2, Mathf.Min(from2, to2), Mathf.Max(from2, to2));
    }
    
    public static int Mod(int a, int n)
    {
        return ((a % n) + n) % n;
    }

    public static float Mod(float a, float n)
    {
        return ((a % n) + n) % n;
    }
}

