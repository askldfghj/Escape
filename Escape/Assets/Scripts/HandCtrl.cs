using UnityEngine;
using System.Collections;

public class HandCtrl : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        {
            if (col.gameObject.tag == "Player")
            {
                Time.timeScale = 0f;
            }
        }
    }
}
