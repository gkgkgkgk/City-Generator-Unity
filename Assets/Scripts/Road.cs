using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : ScriptableObject
{
    public List<Vector3> points = new List<Vector3>();
    public string name;
    public bool major = true;
    public int id;

    public void addPoint(Vector3 point)
    {
        points.Add(point);
    }
}
