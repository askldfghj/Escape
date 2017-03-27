using UnityEngine;
using System.Collections;

public class FixMinimapPosi : MonoBehaviour {

    public Transform _parentPosi;

	// Use this for initialization
	void Start ()
    {
        float x = -_parentPosi.localPosition.x;
        float y = -_parentPosi.localPosition.y;
        Vector2 _newPosi = new Vector2(x, y);
        transform.localPosition = _newPosi;
	}
}
