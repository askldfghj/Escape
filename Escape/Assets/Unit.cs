using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = 0.4f;

    Transform _target;
    Vector3 _lostLocation;
    Collider2D[] companions;

    public Transform _myPath;
    Transform[] _myPathList;
    int _myPathindex;

    public float speed = 0.5f;
    public float turnDst = 1f;
    public float turnSpeed = 3f;

    public LayerMask _doorMask;
    public LayerMask _companionMask;
    GameMasterScript _gm;
    

    Path path;
    

    enum MoveStatus { Normal = 0, doubt, Chase, Missing, Check, Round, Back }
    MoveStatus _movestat;
    public int sample;

    bool _patroller = false;
    int _magnification;
    bool _isnewDoubting;
    

    void Awake()
    {
        _gm = GameObject.Find("GameMaster").GetComponent<GameMasterScript>();
        _isnewDoubting = true;
        _magnification = 1;
        _myPathindex = 2;
        _lostLocation = Vector3.zero;
        _movestat = MoveStatus.Normal;
        if (_myPath != null)
        {
            _myPathList = _myPath.GetComponentsInChildren<Transform>();
            _patroller = true;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (_patroller)
        {
            StartCoroutine("NormalMove", _myPathindex);
        }
    }

    void FixedUpdate()
    {
        sample = (int)_movestat;
    }

    IEnumerator NormalMove(int index)
    {
        yield return new WaitForSeconds(0.5f);
        PathRequestManager.RequestPath(new PathRequest(transform.position, _myPathList[index].position, OnPathFound));
    }

    IEnumerator ChaseMove()
    {
        while (true)
        {
            LookAt2DLocal(_target.localPosition);
            transform.Translate(Vector3.right * Time.deltaTime * speed);
            yield return null;
        }
    }

    IEnumerator LastPosition(float time)
    {
        yield return new WaitForSeconds(time);
        _lostLocation = _target.position;
        PathRequestManager.RequestPath(new PathRequest(transform.position, _lostLocation, OnPathFound));
    }

    IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(0.5f);
        int count = 0;
        float current_angle = transform.eulerAngles.z;
        float angle = current_angle + 70;
        while (count < 100)
        {
            Quaternion tarrot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, (turnSpeed * 0.8f) * Time.deltaTime);
            count++;
            yield return null;
        }
        count = 0;
        angle = current_angle - 70;
        while (count < 100)
        {
            Quaternion tarrot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, (turnSpeed * 0.8f) * Time.deltaTime);
            count++;
            yield return null;
        }

        _gm.SetTimer();

        Collider2D doorCollider = Physics2D.OverlapCircle(_lostLocation, 1.2f, _doorMask);
        if (doorCollider != null && doorCollider.transform.parent.tag == "Room")
        {
            Vector3 roomPosi = doorCollider.transform.parent.position;
            PathRequestManager.RequestPath(new PathRequest(transform.position, roomPosi, OnPathFound));
        }
        else
        {
            _movestat = MoveStatus.Normal;
            companions = Physics2D.OverlapCircleAll(transform.position, 3f, _companionMask);
            if (companions != null)
            {
                for (int i = 0; i < companions.Length; i++)
                {
                    SightCtrl sight = companions[i].transform.parent.GetComponent<SightCtrl>();
                    sight.SetFind();
                }
            }
            StartCoroutine("NormalMove", _myPathindex);
        }
    }

    IEnumerator RoundMove()
    {
        yield return new WaitForSeconds(0.5f);
        int count = 0;
        float current_angle = transform.eulerAngles.z;
        float angle = current_angle + 70;
        while (count < 100)
        {
            Quaternion tarrot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, (turnSpeed * 0.8f) * Time.deltaTime);
            count++;
            yield return null;
        }
        count = 0;
        angle = current_angle - 70;
        while (count < 100)
        {
            Quaternion tarrot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, (turnSpeed * 0.8f) * Time.deltaTime);
            count++;
            yield return null;
        }
        count = 0;
        angle = current_angle - 180;
        while (count < 100)
        {
            Quaternion tarrot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, (turnSpeed * 0.8f) * Time.deltaTime);
            count++;
            yield return null;
        }

        _movestat = MoveStatus.Normal;
        companions = Physics2D.OverlapCircleAll(transform.position, 3f, _companionMask);
        if(companions != null)
        {
            for (int i = 0; i < companions.Length; i++)
            {
                SightCtrl sight = companions[i].transform.parent.GetComponent<SightCtrl>();
                sight.SetFind();
            }
        }
        StartCoroutine("NormalMove", _myPathindex);
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
        PathRequestManager.RequestPath(new PathRequest(transform.position, _target.position, OnPathFound));
        WaitForSeconds sec = new WaitForSeconds(minPathUpdateTime);
        while (true)
        {
            yield return sec;
            PathRequestManager.RequestPath(new PathRequest(transform.position, _target.position, OnPathFound));
            
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
        if (_movestat == MoveStatus.Normal)
        {
            if (_myPathindex >= _myPathList.Length - 2)
            {
                _myPathindex = 1;
            }
            else
            {
                _myPathindex++;
            }
            StartCoroutine("NormalMove", _myPathindex);
        }
        else if (_movestat == MoveStatus.Missing)
        {
            _movestat = MoveStatus.Check;
            StartCoroutine("CheckMove");
        }
        else if (_movestat == MoveStatus.Check)
        {
            _movestat = MoveStatus.Round;
            StartCoroutine("RoundMove");
        }
        else if (_movestat == MoveStatus.doubt)
        {
            StartCoroutine("DoubtMove2");
        }
    }

    IEnumerator DoubtMove1(Vector3 targetPosi)
    {
        float angle = Mathf.Atan2(targetPosi.y - transform.position.y,
                                  targetPosi.x - transform.position.x) * Mathf.Rad2Deg;
        int count = 0;
        while (count < 100)
        {
            Quaternion tarrot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, (turnSpeed * 0.8f) * Time.deltaTime);
            count++;
            yield return null;
        }
        StartCoroutine("LastPosition", 0f);
    }

    IEnumerator DoubtMove2()
    {
        int count = 0;
        float current_angle = transform.eulerAngles.z;
        float angle = current_angle + 70;
        while (count < 100)
        {
            Quaternion tarrot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, (turnSpeed * 0.8f) * Time.deltaTime);
            count++;
            yield return null;
        }
        count = 0;
        angle = current_angle - 70;
        while (count < 100)
        {
            Quaternion tarrot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, (turnSpeed * 0.8f) * Time.deltaTime);
            count++;
            yield return null;
        }

        _movestat = MoveStatus.Normal;
        StartCoroutine("NormalMove", _myPathindex);
    }

    IEnumerator DoubtingDelay()
    {
        yield return new WaitForSeconds(1f);
        _isnewDoubting = true;
    }

    void LookAt2DLocal(Vector3 targetPosi)
    {
        float angle = Mathf.Atan2(targetPosi.y - transform.localPosition.y,
                                  targetPosi.x - transform.localPosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetChase(Transform target)
    {
        if (_movestat != MoveStatus.Chase)
        {
            _target = target;
            StopAllCoroutines();
            _isnewDoubting = true;
            StartCoroutine("UpdatePath");
            _movestat = MoveStatus.Chase;
        }
    }

    public void SetDoubt(Transform target)
    {
        if (_isnewDoubting)
        {
            _isnewDoubting = false;
            _magnification++;
            _target = target;
            StopAllCoroutines();
            _movestat = MoveStatus.doubt;
            StartCoroutine("DoubtMove1", _target.position);
            StartCoroutine("DoubtingDelay");
        }
    }

    public void SetMissing(Vector3 lostposi)
    {
        if (_movestat != MoveStatus.Missing)
        {
            StopCoroutine("UpdatePath");
            StartCoroutine("LastPosition", 0.75f);
            _movestat = MoveStatus.Missing;
        }
    }

    public int GetMagnification()
    {
        return _magnification;
    }

    float toPositive(float a)
    {
        if (a < 0)
        {
            return -a;
        }
        return a;
    }

    //public void OnDrawGizmos()
    //{
    //    Vector3 vec = new Vector3(0.05f, 0.05f, 0.05f);
    //    if (path != null)
    //    {
    //        path.DrawWithGizmos();           
    //    }
    //} 
 }
