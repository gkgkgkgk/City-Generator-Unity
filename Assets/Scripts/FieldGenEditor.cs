using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldGen))]
public class FieldGenEditor : Editor
{
    bool draw = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FieldGen gen = (FieldGen)target;
        if (draw && GUILayout.Button("Draw Tensors"))
        {
            gen.drawTensors();
            draw = false;
        }
        else if (!draw && GUILayout.Button("Hide Tensors"))
        {
            gen.hideTensors();
            draw = true;
        }
    }
}
