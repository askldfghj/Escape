using UnityEngine;
using System.Collections;

public class BasicEnemyCtrl : MonoBehaviour
{
    Transform _target;
    public Vector3 direction;
    public float _velocity;
    public Vector3 _lostLocation;

    enum MoveStatus { Normal = 0, Chase, Missing }

    MoveStatus _movestat;


    void Awake()
    {
        _lostLocation = Vector3.zero;
        _movestat = MoveStatus.Normal;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float distance = 0;
        switch (_movestat)
        {
            case MoveStatus.Normal:
                break;
            case MoveStatus.Chase:
                LooAt2D(_target.localPosition);
                direction = (_target.localPosition - transform.localPosition).normalized;
                distance = Vector3.Distance(_target.localPosition, transform.localPosition);
                transform.Translate(Vector3.right * Time.deltaTime * 0.5f);
                break;
            case MoveStatus.Missing:
                break;
        }
    }

    void LooAt2D(Vector3 targetPosi)
    {
        float angle = Mathf.Atan2(targetPosi.y - transform.localPosition.y,
                                  targetPosi.x - transform.localPosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void DetectPlayer()
    {
        _movestat = MoveStatus.Chase;
    }

    public void SetNormal()
    {
        _movestat = MoveStatus.Normal;
    }

    public void SetChase(Transform target)
    {
        _movestat = MoveStatus.Chase;
        _target = target;
    }

    public void SetMissing()
    {
        _movestat = MoveStatus.Missing;
    }
}
