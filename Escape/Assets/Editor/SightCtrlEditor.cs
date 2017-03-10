using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (SightCtrl))]
public class SightCtrlEditor : Editor
{
    void OnSceneGUI()
    {
        SightCtrl sightctrl = (SightCtrl)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(sightctrl.transform.position, Vector3.forward, Vector3.up, 360, sightctrl.viewRadius);
        Handles.DrawWireArc(sightctrl.transform.position, Vector3.forward, Vector3.up, 360, sightctrl._maxViewRadius);
        Vector3 viewAngleA = sightctrl.DirFromAngle(-sightctrl.viewAngle / 2, false);
        Vector3 viewAngleB = sightctrl.DirFromAngle(sightctrl.viewAngle / 2, false);

        Handles.DrawLine(sightctrl.transform.position, sightctrl.transform.position + viewAngleA * sightctrl._maxViewRadius);
        Handles.DrawLine(sightctrl.transform.position, sightctrl.transform.position + viewAngleB * sightctrl._maxViewRadius);

        Handles.color = Color.red;
        if (sightctrl._target != null)
        {
            Handles.DrawLine(sightctrl.transform.position, sightctrl._target.position);
        }
    }
}
