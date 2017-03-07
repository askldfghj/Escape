using UnityEngine;
using System.Collections;

public class E_eyeScript : MonoBehaviour
{
    public BasicEnemyCtrl _eCtrl;

    void OnTriggerEnter2D(Collider2D col)
    {
        _eCtrl.DetectPlayer();
    }
}
