using UnityEngine;
using System.Collections;

public class ExitDoorCtrl : MonoBehaviour
{
    GameMasterScript _gm;

    void Awake()
    {
        _gm = GameObject.Find("GameMaster").GetComponent<GameMasterScript>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Time.timeScale = 0f;
            _gm.OnGameOver();
        }
    }
}
