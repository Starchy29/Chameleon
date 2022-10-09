using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfViewScript))]
public class FieldOfViewEditorScript : Editor
{

    void OnSceneGUI()
    {
        FieldOfViewScript fow = (FieldOfViewScript)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.right, 360, fow.viewRadius);
        //Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.right, 360, Mathf.Infinity);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        //Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * Mathf.Infinity);
        //Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * Mathf.Infinity);

        Handles.color = Color.red;
        foreach(Transform visibleTarget in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}