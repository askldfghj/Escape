using UnityEngine;
using System.Collections;

public class PlayerCtrl : MonoBehaviour
{
    // Use this for initialization
    public GameObject _minimap;

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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Time.timeScale = 0f;
        }
    }
}
