using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FieldOfViewScript : MonoBehaviour
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

    public enum EnemyMovement // I would make a BirdMovement enum and SnakeMovement
    {
        FullSnakeRight,
        FullSnakeLeft,
        HalfSnakeRight,
        HalfSnakeLeft,
        FaceNorth,
        FaceEast,
        FaceSouth,
        FaceWest,
        BirdMovement
    }
    
    [SerializeField] public EnemyMovement enemyMovement = EnemyMovement.FullSnakeRight; //enum for how the enemies move

    [Range(0, 360)] public float viewAngle;     // How wide the field of view is
    [Range(0, 360)] public float fovRotation;   // Current angle of rotation

    public float innerViewRadius = 3;   // Player must stand still
    public float outerViewRadius = 6;   // Player can move
    public float meshResolution = 10;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();

    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    public bool targetAquired = false;

    private bool lookingRight = true;
    private bool lookingLeft = true;

    [SerializeField] private Transform[] waypoints; // Array of waypoints
    [SerializeField] private float moveSpeed = 2f;  // Walk speed
    [SerializeField] private int waypointIndex = 0; // Index of current waypoint 

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetsWithDelay", .2f);

        if(waypoints.Length > 0)
        {
            transform.position = waypoints[waypointIndex].transform.position;
        }
    }

    private void Update()
    {
        DrawFieldOfView();

        //FullSnakeRightMovement();

        //moves the enemy and cone of vision depending on the enum
        switch(enemyMovement)
        {
            case EnemyMovement.FullSnakeRight:
                FullSnakeRightMovement();
                break;
            case EnemyMovement.FullSnakeLeft:
                FullSnakeLeftMovement();
                break;
            case EnemyMovement.HalfSnakeRight:
                HalfSnakeRightMovement();
                break;
            case EnemyMovement.HalfSnakeLeft:
                HalfSnakeLeftMovement();
                break;
            case EnemyMovement.FaceNorth:
                fovRotation = 0;
                break;
            case EnemyMovement.FaceEast:
                fovRotation = 90;
                break;
            case EnemyMovement.FaceSouth:
                fovRotation = 180;
                break;
            case EnemyMovement.FaceWest:
                fovRotation = 270;
                break;
            case EnemyMovement.BirdMovement:
                BirdMovement();
                break;

        }

        // Checks if target is aquired
        if (targetAquired)
        {
            // Instead of triggering reset it will trigger behavior change for enemy - attack
            GameManager.Instance.RestartLevel(); // Reset scene
        }
    }

    // -- Detection Methods -- //
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    //detects any set targets
    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider2D[] targetsInOuterViewRadius = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), outerViewRadius, targetMask);
        Collider2D[] targetsInInnerViewRadius = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), innerViewRadius, targetMask); // can probably be calculated with outer objects

        for (int i = 0; i < targetsInOuterViewRadius.Length; i++)
        {
            Transform target = targetsInOuterViewRadius[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            
            // What does this do? Gets angle to target?
            //if (Vector2.Angle(new Vector2(Mathf.Sin(fovRotation * Mathf.Deg2Rad), Mathf.Cos(fovRotation * Mathf.Deg2Rad)), dirToTarget) < viewAngle / 2)
            //{
            //    float distToTarget = Vector3.Distance(transform.position, target.position);

            //    if (!Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
            //    {
            //        visibleTargets.Add(target);

            //        // Check if chameleon is visable & if object is the chameleon
            //        ChameleonScript chameleonScript = target.gameObject.GetComponent<ChameleonScript>();
            //        if (chameleonScript != null)
            //        {
            //            if (chameleonScript.Visible)
            //            {
            //                targetAquired = true;
            //            }
            //            else { targetAquired = false; }
            //        }
            //        else { targetAquired = false; }
            //    }
            //}
        }
        for (int i = 0; i < targetsInInnerViewRadius.Length; i++)
        {
            Transform target = targetsInInnerViewRadius[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            if (Vector2.Angle(new Vector2(Mathf.Sin(fovRotation * Mathf.Deg2Rad), Mathf.Cos(fovRotation * Mathf.Deg2Rad)), dirToTarget) < viewAngle /2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);

                    // Check if chameleon is visable & if object is the chameleon
                    ChameleonScript chameleonScript = target.gameObject.GetComponent<ChameleonScript>();
                    if (chameleonScript != null)
                    {
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

    // -- Bird Movements -- //
    /// <summary>
    /// Movement for the bird
    /// </summary>
    private void BirdMovement()
    {
        if (waypointIndex < waypoints.Length)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].transform.position
                , moveSpeed * Time.deltaTime);

            //changes waypoints after getting within a certain distance
            if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 3)
            {
                waypointIndex += 1;
                if (waypointIndex == waypoints.Length)
                {
                    waypointIndex = 0;
                }
            }

            //rotate upon reaching a new waypoint
            Vector3 targetinVec3 = new Vector3(waypoints[waypointIndex].transform.position.x,
                waypoints[waypointIndex].transform.position.y, 0f);

            Vector3 direction2 = targetinVec3 - transform.position;
            float angle = Mathf.Atan2(direction2.y, direction2.x) * Mathf.Rad2Deg - 90;

            //rotates the bird to look at the next waypoint
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction2);
            transform.rotation = rotation;

            //transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex].transform.position,
            //    moveSpeed * Time.deltaTime);

            fovRotation = angle;
        }
    }

    // -- Snake Movements -- //
    /// <summary>
    /// Rotates the snake to the right in a 360 degree angle
    /// </summary>
    private void FullSnakeRightMovement()
    {
        fovRotation += 0.2f;
        if (fovRotation >= 360)
        {
            fovRotation = 0;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, -fovRotation);
        transform.rotation = rotation;
    }

    /// <summary>
    /// rotates the snake to the left in a 360 degree angle
    /// </summary>
    private void FullSnakeLeftMovement()
    {
        fovRotation -= 0.2f;
        if (fovRotation <= -360)
        {
            fovRotation = 0;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, -fovRotation);
        transform.rotation = rotation;
    }

    /// <summary>
    /// rotates the snake to the right in a 180 degree angle
    /// </summary>
    private void HalfSnakeRightMovement()
    {
        if(lookingRight == true)
        {
            fovRotation += 0.2f;
        }
        if(lookingRight == false)
        {
            fovRotation -= 0.2f;
        }
   
        if (fovRotation > 180)
        {
            lookingRight = false;
        }
        if(fovRotation < 0)
        {
            lookingRight = true;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, -fovRotation);
        transform.rotation = rotation;
    }

    /// <summary>
    /// rotates the snake to the left in a 180 degree angle
    /// </summary>
    private void HalfSnakeLeftMovement()
    {
        if (lookingLeft == true)
        {
            fovRotation -= 0.2f;
        }
        if (lookingLeft == false)
        {
            fovRotation += 0.2f;
        }

        if (fovRotation < -180)
        {
            lookingLeft = false;
        }
        if (fovRotation >= 0)
        {
            lookingLeft = true;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, -fovRotation);
        transform.rotation = rotation;
    }

    // -- Visuals -- //
    /// <summary>
    /// draws the field of vision
    /// </summary>
    void DrawFieldOfView()
    {
        int rayCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / rayCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < rayCount; i++)
        {
            float angle = fovRotation - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewInnerCast(angle);

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
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * innerViewRadius, Color.red);
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

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewInnerCast(angle);

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

    ViewCastInfo ViewInnerCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position, dir, innerViewRadius, obstacleMask);

        if (hit.collider != null)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * innerViewRadius, innerViewRadius, globalAngle);
        }
    }
    ViewCastInfo ViewOuterCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position, dir, outerViewRadius, obstacleMask);

        if (hit.collider != null)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * outerViewRadius, outerViewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        //converts angle to global
        if (!angleIsGlobal)
        {
            angleInDegrees += fovRotation;
        }
        //return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}
