using UnityEngine;
using System.Collections;

public class GameMasterScript : MonoBehaviour
{
    public UILabel _timerLabel;
    GameObject _timerObject;
    float _timer;

    bool _timerOn;
    // Use this for initialization
    void Awake()
    {
        _timerOn = false;
        _timerObject = _timerLabel.gameObject;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_timerOn)
        {
            _timerLabel.text = string.Format("{0:f2}", _timer);
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                _timerOn = false;
                _timerObject.SetActive(false);
            }
        }
    }

    public void SetTimer()
    {
        if (!_timerOn)
        {
            _timer = 99.99f;
            _timerOn = true;
            _timerObject.SetActive(true);
            _timerLabel.text = _timer.ToString();
        }
    }

    public void ReTimer()
    {
        if (_timerOn)
        {
            _timer = 99.99f;
        }
    }
}
