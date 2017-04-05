using UnityEngine;
using System.Collections;

public class KeyContrl : MonoBehaviour
{
    DoorCtrl _parentDoor;

    void Awake()
    {
        _parentDoor = transform.parent.GetComponent<DoorCtrl>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            _parentDoor.OpenTheDoor();
            gameObject.SetActive(false);
        }
    }
}
