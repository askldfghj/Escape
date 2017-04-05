using UnityEngine;
using System.Collections;

public class DoorCtrl : MonoBehaviour {

    public UISprite _sprite;
    BoxCollider2D _collider;
    Color _color;
	// Use this for initialization
	void Awake ()
    {
        _color = Color.blue;
        _collider = GetComponent<BoxCollider2D>();
	}

    public void OpenTheDoor()
    {
        _sprite.color = _color;
        _collider.isTrigger = true;
    }
}
