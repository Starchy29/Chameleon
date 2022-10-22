using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FieldOfViewScript : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    [Range(0, 360)]
    public float fovRotation;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;

    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    public bool targetAquired = false;

    bool lookingRight = true;
    bool lookingLeft = true;




    //array of waypoints
    [SerializeField]
    private Transform[] waypoints;

    //walk speed
    [SerializeField]
    private float moveSpeed = 2f;

    private int waypointIndex = 0;

    //enum for how the enemies move
    [SerializeField] public EnemyMovement enemyMovement = EnemyMovement.FullSnakeRight;

    public enum EnemyMovement
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

    //// Start is called before the first frame update
    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetsWithDelay", .2f);

        transform.position = waypoints[waypointIndex].transform.position;
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    // Update is called once per frame
    private void LateUpdate()
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

        // Reset scene
        if (targetAquired)
        {
            GameManager.Instance.RestartLevel();
        }
    }

    //movement for the bird
    private void BirdMovement()
    {
        if (waypointIndex <= waypoints.Length - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].transform.position
                , moveSpeed * Time.deltaTime);

            //changes waypoints after getting within a certain distance
            if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 3)
            {
                waypointIndex += 1;
            }

            //rotate upon reaching a new waypoint
            Vector3 targetinVec3 = new Vector3(waypoints[waypointIndex].transform.position.x,
                waypoints[waypointIndex].transform.position.y, 0f);

            Vector3 direction2 = targetinVec3 - transform.position;
            float angle = Mathf.Atan2(direction2.y, direction2.x) * Mathf.Rad2Deg - 90;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

           // transform.rotation = rotation;

            //transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex].transform.position,
            //    moveSpeed * Time.deltaTime);

            fovRotation = angle;
        }

        if (waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    //rotates the snake to the right in a 360 degree angle
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

    //rotates the snake to the left in a 360 degree angle
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

    //rotates the snake to the right in a 180 degree angle
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

    //rotates the snake to the left in a 180 degree angle
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


    //detects any set targets
    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            if (Vector2.Angle(new Vector2(Mathf.Sin(fovRotation * Mathf.Deg2Rad), Mathf.Cos(fovRotation * Mathf.Deg2Rad)), dirToTarget) < viewAngle / 2)
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

    //draws the field of vision
    void DrawFieldOfView()
    {
        int rayCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / rayCount;
        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < rayCount; i++)
        {
            float angle = fovRotation - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

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
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
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
            ViewCastInfo newViewCast = ViewCast(angle);

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

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

        if (hit.collider != null)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
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
}
