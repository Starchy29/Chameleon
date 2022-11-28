using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SnakeFOV : MonoBehaviour
{
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
    
    [SerializeField] [Range(0, 360)] private float viewAngle = 60f;     // How wide the field of view is
    [SerializeField] [Range(0, 360)] private float fovRotation = 0f;   // Current angle of rotation

    [SerializeField] [Range(0, 10)] private float innerRadius = 3;   // Player must stand still
    [SerializeField] [Range(0, 10)] private float outerRadius = 6;   // Player can move
    private float meshResolution = 10;
    private int edgeResolveIterations;
    private float edgeDistanceThreshold;

    [SerializeField] private LayerMask targetMask;    // Targets in the cone vision
    [SerializeField] private LayerMask obstacleMask;  //Objects that block the cone vision
    private bool targetAquired;
    private List<Transform> visibleTargetsInner;
    private List<Transform> visibleTargetsOuter;
    private SnakeMovement snakeMovement;
    private GameObject chameleon;
    private bool prevTargetStatus = false;

    private Mesh viewMesh;
    public MeshFilter viewMeshFilter;

    public float Rotation
    {
        get { return fovRotation; }
        set { fovRotation = value; }
    }
    public float ViewAngle
    {
        get { return viewAngle; }
    }
    public float InnerRadius
    {
        get { return innerRadius; }
    }
    public float OuterRadius
    {
        get { return outerRadius; }
    }

    private void Start()
    {
        visibleTargetsInner = new List<Transform>();
        visibleTargetsOuter = new List<Transform>();
        targetAquired = false;
        snakeMovement = GetComponent<SnakeMovement>();

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter = GetComponentInChildren<MeshFilter>();
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    private void Update()
    {
        DrawFieldOfView();

        // Checks if target is aquired
        if (targetAquired)
        {
            // Instead of triggering reset it will trigger behavior change for enemy - attack
            snakeMovement.SpottedEnemy(chameleon);
        }
        else
        {
            //if (prevTargetStatus)
            //{
            //    birdMovement.LostEnemy();
            //}
        }
        prevTargetStatus = targetAquired;
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            CheckOuterCircle();
        }
    }
    private void CheckOuterCircle()
    {
        visibleTargetsOuter.Clear();

        Collider2D[] targetsInOuterRadius = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), outerRadius, targetMask);

        // For any targets within the outer radius
        for (int i = 0; i < targetsInOuterRadius.Length; i++)
        {
            Transform target = targetsInOuterRadius[i].transform;

            // Direction to Target
            Vector2 dirToTarget = (target.position - transform.position).normalized;

            // Checks the SIDES of the vision cone
            if (Vector2.Angle(new Vector2(Mathf.Sin(fovRotation * Mathf.Deg2Rad), Mathf.Cos(fovRotation * Mathf.Deg2Rad)), dirToTarget) < viewAngle /2)
            {
                // Distance between enemy and target
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargetsOuter.Add(target);

                    // Check if chameleon is visable & if object is the chameleon
                    ChameleonScript chameleonScript = target.gameObject.GetComponent<ChameleonScript>();
                    if (chameleonScript != null)
                    {
                        chameleon = chameleonScript.gameObject;
                        if (chameleonScript.Visible)
                        {
                            targetAquired = true;
                        }
                        else { targetAquired = false; }
                    }
                    else { targetAquired = false; }
                }
            }
        }
    }
    private bool CheckInnerCircle()
    {
        visibleTargetsInner.Clear();

        Collider2D[] targetsInInnerRadius = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), innerRadius, targetMask);

        // For any targets within the outer radius
        for (int i = 0; i < targetsInInnerRadius.Length; i++)
        {
            Transform target = targetsInInnerRadius[i].transform;

            // Direction to Target
            Vector2 dirToTarget = (target.position - transform.position).normalized;

            // Checks the SIDES of the vision cone
            if (Vector2.Angle(new Vector2(Mathf.Sin(fovRotation * Mathf.Deg2Rad), Mathf.Cos(fovRotation * Mathf.Deg2Rad)), dirToTarget) < viewAngle / 2)
            {
                // Distance between enemy and target
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargetsInner.Add(target);

                    // Check if chameleon is visable & if object is the chameleon
                    ChameleonScript chameleonScript = target.gameObject.GetComponent<ChameleonScript>();
                    if (chameleonScript != null)
                    {
                        chameleon = chameleonScript.gameObject;
                        if (chameleonScript.Visible)
                        {
                            return true;
                        }
                        else { return false; }
                    }
                }
            }
        }
        return false;
    }
    private void DrawFieldOfView()
    {
        int rayCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / rayCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < rayCount; i++)
        {
            float angle = fovRotation - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle, outerRadius);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle, outerRadius);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDistanceThreshold;

            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }
    private ViewCastInfo ViewCast(float globalAngle, float radius)
    {
        Vector3 dir = DirectionFromAngle(globalAngle, true);
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position, dir, radius, obstacleMask);

        if (hit.collider != null)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * radius, radius, globalAngle);
        }
    }
    
    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        // Converts angle to global
        if (!angleIsGlobal)
        {
            angleInDegrees += fovRotation;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmosSelected()
    {
        Transform birdTransform = gameObject.transform;

        Gizmos.color = Color.white;

        // Draw Circles
        Gizmos.DrawWireSphere(transform.position, innerRadius);
        Gizmos.DrawWireSphere(transform.position, outerRadius);

        //Gizmos.DrawWireArc(birdTransform.position, birdTransform.forward, birdTransform.right, 360, birdFOV.InnerRadius);
        //Gizmos.DrawWireArc(birdTransform.position, birdTransform.forward, birdTransform.right, 360, birdFOV.OuterRadius);

        // Get Line Angles
        Vector3 viewAngleA = DirectionFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirectionFromAngle(ViewAngle / 2, false);

        // Draw Lines
        Gizmos.DrawLine(birdTransform.position, birdTransform.position + viewAngleA * outerRadius);
        Gizmos.DrawLine(birdTransform.position, birdTransform.position + viewAngleB * outerRadius);
    }
}
