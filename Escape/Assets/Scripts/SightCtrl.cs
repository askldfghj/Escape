using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SightCtrl : MonoBehaviour
{
    [Range(0, 3)]
    public float _minRadius;
    [Range(0, 3)]
    public float _maxViewRadius;
    [Range(0, 360)]
    public float _minViewAngle;
    [Range(0, 360)]
    public float _maxViewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public Transform _target;
    Transform _oldtarget;
    [Range(0, 1)]
    public float meshResolution;
    public int edgeResolveInterations;
    public float edgeDstThreshold;

    public Renderer _mRenderer;

    List<Vector3> viewPoints;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    float _currentRadius;
    float _currentViewAngle;

    ViewCastInfo oldViewCast;
    ViewCastInfo newViewCast;

    Collider2D _targetCollider;

    //이후 추가 변수
    bool _isCatch;
    //public BasicEnemyCtrl _eCtrl;
    public Unit _eUnit;

    void Awake()
    {
        _isCatch = false;
       viewPoints  = new List<Vector3>();
        _currentRadius = _minRadius;
        _currentViewAngle = _minViewAngle;
    }

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetsWithDelay", .2f);
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        WaitForSeconds sec = new WaitForSeconds(delay);
        while (true)
        {
            yield return sec;
            FindVisibleTargets();
        }
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void FindVisibleTargets()
    {

        //아직 발견 못함.
        if (!_isCatch)
        {
            _target = null;
            _targetCollider = Physics2D.OverlapCircle(transform.position, _currentRadius, targetMask);
            if (_targetCollider != null)
            {
                Transform target = _targetCollider.transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.right, dirToTarget) < _currentViewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        _target = target;
                        _eUnit.SetChase(_target);
                        _isCatch = true;
                        Color color = new Color(1, 0.92f, 0.016f, 0.5f);
                        _mRenderer.material.color = color;
                        _currentRadius = _maxViewRadius;
                        _currentViewAngle = _maxViewAngle;
                    }
                }
            }
        }
        //발견후 쫓아갈때.
        else
        {
            _target = null;
            _targetCollider = Physics2D.OverlapCircle(transform.position, _currentRadius, targetMask);
            if (_targetCollider != null)
            {
                Transform target = _targetCollider.transform;
                _oldtarget = _targetCollider.transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                _target = target;
                if (Vector3.Angle(transform.right, dirToTarget) < _currentViewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        
                    }
                    else
                    {
                        _eUnit.SetMissing(_target.position);
                        _isCatch = false;
                    }
                }
                else
                {
                    _eUnit.SetMissing(_target.position);
                    _isCatch = false;
                }
            }
            else
            {
                _eUnit.SetMissing(_oldtarget.position);
                _isCatch = false;
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(_minViewAngle * meshResolution);
        float stepAngleSize = _minViewAngle / stepCount;
        viewPoints.Clear();
        oldViewCast.Init();
        newViewCast.Init();

        float angle;
        bool edgeDstThresholdExceeded;
        EdgeInfo edge;
        for (int i = 0; i <= stepCount; i++)
        {
            angle = transform.eulerAngles.z - _minViewAngle / 2 + stepAngleSize * i;
            newViewCast = ViewCast(angle);

            if (i > 0)
            {
                edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    edge = FindEdge(oldViewCast, newViewCast);
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
        //viewMesh.RecalculateNormals();
    }


    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveInterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && edgeDstThresholdExceeded)
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
        hit = Physics2D.Raycast(transform.position, dir, _minRadius, obstacleMask);
        if (hit.collider != null)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * _minRadius, _minRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
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

        public void Init()
        {
            hit = false;
            point = Vector3.zero;
            dst = 0f;
            angle = 0f;
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
