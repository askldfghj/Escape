using UnityEngine;
using System.Collections;

public class HandCtrl : MonoBehaviour
{
    GameMasterScript _gm;
    void Awake()
    {
        _gm = GameObject.Find("GameMaster").GetComponent<GameMasterScript>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        {
            if (col.gameObject.tag == "Player")
            {
                Time.timeScale = 0f;
                _gm.OnGameOver();
            }
        }
    }
}
