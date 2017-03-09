using UnityEngine;
using System.Collections;

public class BasicEnemyCtrl : MonoBehaviour
{
    public Transform _player;
    public Transform _eyePosi;
    public Vector3 direction;
    public PolygonCollider2D _eyecol;
    public float _velocity;
    public Vector3 _lostLocation;
    RaycastHit2D _ray2d;

    bool _isChased;
    bool _isMissing;

    int _layerMask;

    int count;

    enum MoveStatus { Normal = 0, Chase, Missing }

    MoveStatus _movestat;


    void Awake()
    {
        count = 0;
        _layerMask = 1 << 5;
        _layerMask = ~_layerMask;
        _lostLocation = Vector3.zero;
        _isChased = false;
        _isMissing = false;
        _movestat = MoveStatus.Normal;
    }


    void Move()
    {
        float distance = 0;
        switch (_movestat)
        {
            case MoveStatus.Normal:
                break;
            case MoveStatus.Chase:
                LooAt2D(_player.localPosition);
                direction = (_player.localPosition - transform.localPosition).normalized;
                distance = Vector3.Distance(_player.localPosition, transform.localPosition);
                transform.Translate(Vector3.right * Time.deltaTime * 0.5f);
                break;
            case MoveStatus.Missing:
                LooAt2D(_lostLocation);
                direction = (_lostLocation - transform.localPosition).normalized;
                distance = Vector3.Distance(_lostLocation, transform.localPosition);
                transform.Translate(Vector3.right * Time.deltaTime * 0.5f);
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
        count++;
        Debug.Log("Player Detect!" + count);
        _movestat = MoveStatus.Chase;
        _isChased = true;
    }
}
