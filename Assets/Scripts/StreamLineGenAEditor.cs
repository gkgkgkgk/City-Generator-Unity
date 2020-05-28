using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StreamLineGenA))]
public class StreamLineGenAEditor : Editor
{
    bool draw = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StreamLineGenA gen = (StreamLineGenA)target;

        if (draw && GUILayout.Button("Draw Roads"))
        {
            gen.drawRoads();
            draw = false;
        }
        else if (!draw && GUILayout.Button("Hide Roads"))
        {
            gen.hideRoads();
            draw = true;
        }

        if (GUILayout.Button("Generate"))
        {
            gen.generate();
        }
    }
}
