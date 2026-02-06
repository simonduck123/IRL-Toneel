using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Rope : MonoBehaviour
{
    [Header("Rope Settings")]
    public int segmentCount = 25;          // more segments = smoother rope
    public float segmentLength = 0.1f;     // vertical distance per segment
    public float segmentRadius = 0.06f;    // capsule radius
    public float segmentMass = 0.25f;      // slightly heavier = stable
    public float angularLimit = 30f;       // max joint bend angle
    public float angularDriveStiffness = 600f;  // moderate stiffness
    public float angularDriveDamping = 50f;     // prevents wild jitter

    [Header("Editor Settings")]
    public bool autoClearOld = true;       // remove previous rope before generating

    public void GenerateRope()
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Generate Rope");
#endif

        // Remove old rope
        if (autoClearOld)
        {
            GameObject[] children = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
                children[i] = transform.GetChild(i).gameObject;

            foreach (var c in children)
            {
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(c);
#else
                Destroy(c);
#endif
            }
        }

        Rigidbody previousRb = null;

        for (int i = 0; i < segmentCount; i++)
        {
            // Create capsule segment
            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            segment.name = "RopeSegment_" + i;
            segment.transform.parent = transform;

            // Position slightly overlapping
            segment.transform.position = transform.position - Vector3.up * segmentLength * i * 0.95f;
            segment.transform.localScale = new Vector3(segmentRadius * 2, segmentLength / 2, segmentRadius * 2);
            segment.transform.rotation = Quaternion.identity;

            // Remove default collider and add normalized capsule collider
            DestroyImmediate(segment.GetComponent<Collider>());
            CapsuleCollider collider = segment.AddComponent<CapsuleCollider>();
            collider.radius = 0.5f;
            collider.height = 1f;
            collider.direction = 1;

            // Rigidbody
            Rigidbody rb = segment.AddComponent<Rigidbody>();
            rb.mass = segmentMass;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.isKinematic = (i == 0); // top segment = anchor

            // Add ConfigurableJoint
            if (i > 0)
            {
                ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
                joint.connectedBody = previousRb;

                // Limited linear motion
                joint.xMotion = ConfigurableJointMotion.Limited;
                joint.yMotion = ConfigurableJointMotion.Limited;
                joint.zMotion = ConfigurableJointMotion.Limited;
                joint.linearLimit = new SoftJointLimit { limit = segmentLength * 1.05f };

                // Limited angular motion
                joint.angularXMotion = ConfigurableJointMotion.Limited;
                joint.angularYMotion = ConfigurableJointMotion.Limited;
                joint.angularZMotion = ConfigurableJointMotion.Limited;

                joint.lowAngularXLimit = new SoftJointLimit { limit = -angularLimit };
                joint.highAngularXLimit = new SoftJointLimit { limit = angularLimit };
                joint.angularYLimit = new SoftJointLimit { limit = angularLimit };
                joint.angularZLimit = new SoftJointLimit { limit = angularLimit };

                // Angular drive for stability
                JointDrive drive = new JointDrive
                {
                    positionSpring = angularDriveStiffness,
                    positionDamper = angularDriveDamping,
                    maximumForce = Mathf.Infinity
                };
                joint.angularXDrive = drive;
                joint.angularYZDrive = drive;

                joint.enableCollision = true;
                joint.projectionMode = JointProjectionMode.PositionAndRotation;
            }

            previousRb = rb;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Rope))]
public class RopeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Rope ropeGen = (Rope)target;

        if (GUILayout.Button("Spawn Rope"))
        {
            ropeGen.GenerateRope();
        }
    }
}
#endif