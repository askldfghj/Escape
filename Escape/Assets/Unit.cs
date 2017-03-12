using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;


    Transform _target;
    Vector3 _lostLocation;
    public Vector3 direction;

    public float speed = 0.5f;
    public float turnDst = 1f;
    public float turnSpeed = 3f;

    Path path;
    

    enum MoveStatus { Normal = 0, Chase, Missing, Check, Round, Back }
    MoveStatus _movestat;

    void Awake()
    {
        _lostLocation = Vector3.zero;
        _movestat = MoveStatus.Normal;
    }

    // Use this for initialization
    void Start()
    {
        //StartCoroutine(UpdatePath());
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        switch (_movestat)
        {
            case MoveStatus.Normal:
                break;
            case MoveStatus.Chase:
                ChaseMove();
                break;
            case MoveStatus.Missing:
                MissingMove();
                break;
        }
    }

    void ChaseMove()
    {
        //LooAt2DLocal(_target.localPosition);
        //direction = (_target.localPosition - transform.localPosition).normalized;
        //transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

    void MissingMove()
    {
        PathRequestManager.RequestPath(new PathRequest(transform.position, _target.position, OnPathFound));
        _movestat = MoveStatus.Check;
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }


    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, _target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = _target.position;
        WaitForSeconds sec = new WaitForSeconds(minPathUpdateTime);
        while (true)
        {
            yield return sec;
            if ((_target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, _target.position, OnPathFound));
                targetPosOld = _target.position;
            }
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;

        Vector2 dir = path.lookPoints[pathIndex] - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion tarRot = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, turnSpeed * Time.deltaTime);

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                dir = path.lookPoints[pathIndex] - transform.position;
                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                tarRot = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, turnSpeed * Time.deltaTime);
                transform.Translate(Vector3.right * Time.deltaTime * speed);

            }
            yield return null;
        }
    }

    void LooAt2D(Vector3 targetPosi)
    {
        float angle = Mathf.Atan2(targetPosi.y - transform.position.y,
                                  targetPosi.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void LooAt2DLocal(Vector3 targetPosi)
    {
        float angle = Mathf.Atan2(targetPosi.y - transform.localPosition.y,
                                  targetPosi.x - transform.localPosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetChase(Transform target)
    {
        _target = target;
        StopCoroutine("FollowPath");
        StopCoroutine("UpdatePath");
        StartCoroutine("UpdatePath");
        _movestat = MoveStatus.Chase;
    }

    public void SetMissing()
    {
        StopCoroutine("UpdatePath");
        _movestat = MoveStatus.Missing;
    }

    public void OnDrawGizmos()
    {
        Vector3 vec = new Vector3(0.05f, 0.05f, 0.05f);
        if (path != null)
        {
            path.DrawWithGizmos();           
        }
    } 
 }
