using UnityEngine;
using System.Collections;

public class PlayerCtrl : MonoBehaviour
{
    // Use this for initialization
    public GameObject _minimap;

    public LayerMask _enemyLayer;

    [Range(0,1)]
    public float _attackRange;

    void Start()
    {
        
    }

    void Update()
    {
        OnMinimap();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        KeyProcess();
    }

    void KeyProcess()
    {
        Move();
    }

    void OnMinimap()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _minimap.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            _minimap.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            HitEnemy();
        }
    }

    void HitEnemy()
    {
        Collider2D targetCollider = Physics2D.OverlapCircle(transform.position, _attackRange, _enemyLayer);
        if (targetCollider != null)
        { 
            Unit enemyUnit = targetCollider.transform.parent.GetComponent<Unit>();
            enemyUnit.SetStun();
        }
    }

    void Move()
    {
        if (!_minimap.activeSelf)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(Vector2.up * 0.5f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(Vector2.down * 0.5f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector2.left * 0.5f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector2.right * 0.5f * Time.deltaTime);
            }
        }
    }
}
