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
        Vector3 viewAngleA = sightctrl.DirFromAngle(-sightctrl.viewAngle / 2, false);
        Vector3 viewAngleB = sightctrl.DirFromAngle(sightctrl.viewAngle / 2, false);

        Handles.DrawLine(sightctrl.transform.position, sightctrl.transform.position + viewAngleA * sightctrl.viewRadius);
        Handles.DrawLine(sightctrl.transform.position, sightctrl.transform.position + viewAngleB * sightctrl.viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in sightctrl.visibleTargets)
        {
            Handles.DrawLine(sightctrl.transform.position, visibleTarget.position);
        }
    }
}
