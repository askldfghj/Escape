using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlayerCtrl))]
public class PlayerEditor : Editor
{
    void OnSceneGUI()
    {
        PlayerCtrl playerctrl = (PlayerCtrl)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(playerctrl.transform.position, Vector3.forward, Vector3.up, 360, playerctrl._attackRange);
    }
}
