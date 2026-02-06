using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RopeGenerator : MonoBehaviour
{
    [Header("Rope Settings")]
    public int segments = 15;
    public float segmentLength = 0.2f;
    public float segmentRadius = 0.05f;
    public float mass = 0.2f;
    public float drag = 0.1f;

    [HideInInspector]
    public Transform[] segmentTransforms;

    public void GenerateRope()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        segmentTransforms = new Transform[segments];

        GameObject prevSegment = null;

        for (int i = 0; i < segments; i++)
        {
            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            segment.name = "RopeSegment_" + i;
            segment.transform.parent = transform;
            segment.transform.localScale = new Vector3(segmentRadius, segmentLength / 2f, segmentRadius);
            segment.transform.position = transform.position - Vector3.up * segmentLength * i;

            Rigidbody rb = segment.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = drag;
            rb.angularDamping = 0.5f;

            CapsuleCollider col = segment.GetComponent<CapsuleCollider>();
            col.direction = 1;

            if (i == 0)
            {
                rb.isKinematic = true;
            }
            else
            {
                HingeJoint joint = segment.AddComponent<HingeJoint>();
                joint.connectedBody = prevSegment.GetComponent<Rigidbody>();
                joint.axis = Vector3.right;
                joint.anchor = new Vector3(0, segmentLength / 2f, 0);
                joint.connectedAnchor = new Vector3(0, -segmentLength / 2f, 0);
                joint.useLimits = true;
                JointLimits limits = joint.limits;
                limits.min = -45;
                limits.max = 45;
                joint.limits = limits;
            }

            segmentTransforms[i] = segment.transform;
            prevSegment = segment;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RopeGenerator))]
public class RopeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RopeGenerator rope = (RopeGenerator)target;
        if (GUILayout.Button("Generate Rope"))
        {
            Undo.RecordObject(rope, "Generate Rope");
            rope.GenerateRope();
            EditorUtility.SetDirty(rope);
        }
    }
}
#endif