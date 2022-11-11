using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfViewScript))]
public class BirdFOVEditor : Editor
{
    void OnSceneGUI()
    {
        //GameObject bird = (GameObject)target;
        //Transform birdTransform = bird.transform;
        //BirdFOV birdFOV = bird.GetComponent<BirdFOV>();

        //Handles.color = Color.white;

        //// Draw Circles
        //Handles.DrawWireArc(birdTransform.position, birdTransform.forward, birdTransform.right, 360, birdFOV.InnerRadius);
        //Handles.DrawWireArc(birdTransform.position, birdTransform.forward, birdTransform.right, 360, birdFOV.OuterRadius);

        //// Get Line Angles
        //Vector3 viewAngleA = birdFOV.DirectionFromAngle(-birdFOV.ViewAngle / 2, false);
        //Vector3 viewAngleB = birdFOV.DirectionFromAngle(birdFOV.ViewAngle / 2, false);

        //// Draw Lines
        //Handles.DrawLine(birdTransform.position, birdTransform.position + viewAngleA * birdFOV.OuterRadius);
        //Handles.DrawLine(birdTransform.position, birdTransform.position + viewAngleB * birdFOV.OuterRadius);
    }
}
