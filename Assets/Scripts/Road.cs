using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : ScriptableObject
{
    public string name;
    public bool major = true;
    public int id;

    public List<Vector3> positivePoints = new List<Vector3>();
    public List<Vector3> negativePoints = new List<Vector3>();
    public List<Vector3> sortedPoints = new List<Vector3>();

    GameObject line;

    public void addPoint(Vector3 point)
    {
        sortedPoints.Add(point);
    }

    public void addPositivePoint(Vector3 point)
    {
        if (!positivePoints.Contains(point))
        {
            positivePoints.Add(point);
        }
    }

    public void addNegativePoint(Vector3 point)
    {
        if (!negativePoints.Contains(point))
        {
            negativePoints.Add(point);
        }
    }

    public void sortPoints()
    {
        negativePoints.Reverse();

        foreach (Vector3 p in negativePoints)
        {
            sortedPoints.Add(p);
        }

        foreach (Vector3 rp in positivePoints)
        {
            sortedPoints.Add(rp);
        }
    }

    public void draw(Material m)
    {
        if (line == null)
        {
            GameObject line = new GameObject();
            line.transform.position = sortedPoints[0];
            line.AddComponent<LineRenderer>();
            LineRenderer lr = line.GetComponent<LineRenderer>();
            int i = 0;
            lr.positionCount = sortedPoints.Count;
            lr.material = m;
            foreach (Vector3 r in sortedPoints)
            {
                lr.SetPosition(i, r);
                i++;
            }
        }
        else
        {
            line.SetActive(true);
        }

    }

    public void hide()
    {
        if (line != null)
        {
            line.SetActive(false);
        }
    }

}
