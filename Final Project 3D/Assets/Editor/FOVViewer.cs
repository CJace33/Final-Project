using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor (typeof (GuardController))]
public class FOVViewer : Editor
{
    private void OnSceneGUI()
    {

        GuardController guard = (GuardController)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(guard.transform.position, Vector3.up, Vector3.forward, 360, guard.viewRadius);
        Vector3 viewAngle1A = guard.DirFromAngle(-guard.viewAngle / 2, false);
        Vector3 viewAngle1B = guard.DirFromAngle(guard.viewAngle / 2, false);
        Handles.DrawLine(guard.transform.position, guard.transform.position + viewAngle1A * guard.viewRadius);
        Handles.DrawLine(guard.transform.position, guard.transform.position + viewAngle1B * guard.viewRadius);


        Handles.color = Color.cyan;
        Handles.DrawWireArc(guard.transform.position, Vector3.up, Vector3.forward, 360, guard.closeViewRadius);
        Vector3 viewAngle2A = guard.DirFromAngle(-guard.closeViewAngle / 2, false);
        Vector3 viewAngle2B = guard.DirFromAngle(guard.closeViewAngle / 2, false);
        Handles.DrawLine(guard.transform.position, guard.transform.position + viewAngle2A * guard.closeViewRadius);
        Handles.DrawLine(guard.transform.position, guard.transform.position + viewAngle2B * guard.closeViewRadius);
    }

}
