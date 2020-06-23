using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamLineGenA : MonoBehaviour
{
    [Tooltip("min distance between seed points")]
    public float dSep = 10f;
    [Tooltip("step distance")]
    public float dStep = 0.5f;

    public List<Road> roads = new List<Road>();

    public Material point;
    public Material nonPoint;

    FieldGen field;

    public float angleThreshold = 3f;

    public Material roadMat;

    void Awake()
    {
        field = this.gameObject.GetComponent<FieldGen>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void generate()
    {
        Vector3 seed = Vector3.zero;
        bool major = true;
        for (int i = 0; i < field.totalSize; i++)
        {
            Road r = buildRoad(seed, major, i);
            if (r != null) { roads.Add(r); }
            major = !major;
            seed = new Vector3(Random.Range(-field.totalSize / 2f, field.totalSize / 2f), 0f, Random.Range(-field.totalSize / 2f, field.totalSize / 2f));
        }
    }

    Road buildRoad(Vector3 seed, bool major, int id)
    {
        Road r = new Road();
        r.id = id;
        r.major = major;

        if (!validPoint(seed, r))
        {
            return null;
        }

        bool buildPos = true;
        bool buildNeg = true;

        Vector3 positionPos = seed;
        Vector3 positionNeg = seed;

        float posTheta = field.sampleTensor(positionPos).theta;
        float negTheta = field.sampleTensor(positionNeg).theta + 180f;

        float fs = 0;

        r.addPositivePoint(seed);

        while (fs < 1000 && buildPos || buildNeg)
        {
            fs++;
            if (buildPos)
            {
                float p = calculateTheta(positionPos, posTheta, major, true);
                Vector3 nextP = rk4nextPos(positionPos, posTheta, major, true);
                positionPos += nextP;

                if (validPoint(positionPos, r))
                {
                    if (Mathf.Abs(p - posTheta) > angleThreshold)
                    {
                        posTheta = p;
                        r.addPositivePoint(positionPos);
                    }
                }
                else
                {
                    r.addPositivePoint(positionPos - nextP);
                    buildPos = false;
                }
            }

            if (buildNeg)
            {
                float n = calculateTheta(positionNeg, negTheta, major, false);
                Vector3 nextN = rk4nextPos(positionNeg, negTheta, major, false);
                positionNeg += nextN;

                if (validPoint(positionNeg, r))
                {
                    if (Mathf.Abs(n - negTheta) > angleThreshold)
                    {
                        negTheta = n;
                        r.addNegativePoint(positionNeg);

                    }
                }
                else
                {
                    r.addNegativePoint(positionNeg - nextN);
                    buildNeg = false;
                }
            }
        }

        r.sortPoints();

        return r;
    }

    public void getIntersections()
    {
        // O (log n) NO.
        foreach (Road r in roads)
        {
            for (int i = 0; i < r.sortedPoints.Count - 1; i++)
            {
                foreach (Road r2 in roads)
                {
                    if (r.id != r2.id)
                    {
                        for (int j = 0; j < r2.sortedPoints.Count - 1; j++)
                        {
                            Vector3 ab = r.sortedPoints[i + 1] - r.sortedPoints[i];
                            Vector3 cd = r2.sortedPoints[i + 1] - r2.sortedPoints[i];

                            // where does ab and cd intersect? add to list.
                        }
                    }
                }
            }
        }
    }


    bool validPoint(Vector3 point, Road road)
    {
        // check for out of bounds
        if (Vector3.Distance(point, Vector3.zero) > field.totalSize / 2f)
        {
            return false;
        }

        foreach (Road road2 in roads)
        {
            if (road.major == road2.major)
                for (int i = 0; i < road2.sortedPoints.Count - 1; i++)
                {

                    Vector3 closest = closestPoint(point, road2.sortedPoints[i], road2.sortedPoints[i + 1]);

                    if (Vector3.Distance(point, closest) < dSep)
                    {
                        return false;
                    }
                }
        }

        // for (int i = 0; i < road.sortedPoints.Count - 1; i++)
        // {

        //     Vector3 closest = closestPoint(point, road.sortedPoints[i], road.sortedPoints[i + 1]);

        //     if (Vector3.Distance(point, closest) < dStep)
        //     {
        //         return false;
        //     }
        // }

        return true;
    }

    Vector3 closestPoint(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector3 ap = p - a;
        Vector3 ab = b - a;
        float m = ((ab.x * ab.x) + (ab.z * ab.z));
        float t = Vector3.Dot(ap, ab) / m;

        Vector3 point = new Vector3(a.x + ab.x * t, 0f, a.z + ab.z * t);

        if (Vector3.Distance(point, a) < Vector3.Distance(a, b) && Vector3.Distance(point, b) < Vector3.Distance(a, b))
        {
            return point;
        }

        return Vector3.Distance(point, a) < Vector3.Distance(point, b) ? a : b;
    }

    Vector3 rk4nextPos(Vector3 p, float previousTheta, bool major, bool pos)
    {
        Vector3 k1 = nextPos(calculateTheta(p, previousTheta, major, pos));
        Vector3 k2 = nextPos(calculateTheta(p + k1 / 2f, previousTheta, major, pos));
        Vector3 k3 = nextPos(calculateTheta(p + k2 / 2f, previousTheta, major, pos));
        Vector3 k4 = nextPos(calculateTheta(p + k3, previousTheta, major, pos));

        return (k1 / 6f) + (k2 / 3f) + (k3 / 3f) + (k4 / 6f);
    }

    Vector3 nextPos(float theta)
    {
        return new Vector3(Mathf.Sin(theta * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(theta * Mathf.Deg2Rad) * dStep);
    }

    float calculateTheta(Vector3 p, float previousTheta, bool major, bool pos)
    {
        float t = field.sampleTensor(p).theta;

        if (major)
        {
            if (!pos)
            {
                t -= 180f;
            }
        }
        else
        {
            if (!pos)
            {
                t -= 90f;
            }
            else
            {
                t += 90f;
            }
        }

        while (t < 0)
            t += 360f;

        if (Mathf.Abs(Mathf.DeltaAngle(previousTheta, t)) > 90f)
        {
            t -= 180f;
        }


        return t;
    }

    public void drawRoads()
    {
        foreach (Road r in roads)
        {
            r.draw(roadMat);
        }
    }

    public void hideRoads()
    {
        foreach (Road r in roads)
        {
            r.hide();
        }
    }

}
