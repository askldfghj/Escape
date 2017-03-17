using UnityEngine;
using System.Collections;

public class ChasePlayer : MonoBehaviour
{
    public GameObject _player;
    Vector3 _offset;
    // Use this for initialization
    void Start()
    {
        _offset = transform.position - _player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _player.transform.position + _offset;
    }
}
