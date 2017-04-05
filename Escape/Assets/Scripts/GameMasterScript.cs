using UnityEngine;
using System.Collections;

public class GameMasterScript : MonoBehaviour
{
    public UILabel _timerLabel;
    public GameObject _enemyPool;

    SightCtrl[] _enemySights;
    GameObject _timerObject;
    float _timer;

    bool _timerOn;
    
    // Use this for initialization
    void Awake()
    {
        _enemySights = _enemyPool.GetComponentsInChildren<SightCtrl>();
        _timerOn = false;
        _timerObject = _timerLabel.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timerOn)
        {
            TimerStart();
        }
    }

    IEnumerator TimerColorLerp()
    {
        float lerptime = 0;
        while (lerptime < 1)
        {
            _timerLabel.color = Color.Lerp(Color.red, Color.white, lerptime);
            lerptime += Time.deltaTime * 0.5f;
            yield return null;
        }
        _timerLabel.color = Color.white;
    }

    void TimerStart()
    {
        _timerLabel.text = string.Format("{0:f2}", _timer);
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _timerOn = false;
            _timerObject.SetActive(false);
            for (int i = 0; i < _enemySights.Length; i++)
            {
                _enemySights[i].InitSightState();
            }
        }
    }

    public void SetTimer()
    {
        if (!_timerOn)
        {
            _timer = 99.99f;
            _timerObject.SetActive(true);
            _timerLabel.color = Color.red;
            _timerLabel.text = _timer.ToString();
        }
        else
        {
            ReTimer();
        }
    }

    public void TimerOn()
    {
        StartCoroutine("TimerColorLerp");
        _timerOn = true;
    }

    public void TimerOff()
    {
        _timerOn = false;
    }

    public void ReTimer()
    {
        _timer = 99.99f;
        StopCoroutine("TimerColorLerp");
        _timerLabel.color = Color.red;
        _timerLabel.text = _timer.ToString();
    }
}
