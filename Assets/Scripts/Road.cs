using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road
{
    public List<Vector3> points = new List<Vector3>();
    public string name;
    public int id;
    public bool major = true;

    public void addPoint(Vector3 point)
    {
        points.Add(point);
    }
}
