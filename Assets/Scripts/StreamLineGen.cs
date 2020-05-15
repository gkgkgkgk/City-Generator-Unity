using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// adapted algorithm from: https://www.researchgate.net/publication/2825680_Creating_Evenly-Spaced_Streamlines_of_Arbitrary_Density
[RequireComponent(typeof(FieldGen))]
public class StreamLineGen : MonoBehaviour
{
    [Tooltip("min distance between seed points")]
    public float dSep = 10f;
    [Tooltip("min distance between streamlines ")]
    public float dTest = 5f; // generally (according to the paper) half of dSep
    [Tooltip("step distance")]
    public float dStep = 0.5f;

    public List<Road> roads = new List<Road>();

    FieldGen field;

    void Awake()
    {
        field = this.gameObject.GetComponent<FieldGen>();
    }

    void Update()
    {

    }

    public void generateStreamLines()
    {
        bool major = true;
        Road r = generateRoad(Vector3.zero, major);

        for (int i = 0; i < 100; i++)
        {
            major = !major;
            Vector3 seed = selectSeed(r);

            r = generateRoad(seed, major);
        }
    }

    Road generateRoad(Vector3 seed, bool major)
    {
        /* notes:
            - all streamlines must be dSep away from each other.
            - sample points in streamlines must be equal distance from each other and smaller than dsep
            - if a streamline gets close to another streamline, it is discontinued in the current direction
            - streamlines start a central point and move in opposite directions
            - new seed points must be at least dSep away from any other streamline on the same axis
        */
        Road r = new Road();
        r.major = major;
        roads.Add(r);

        bool buildPos = true;
        bool buildNeg = true;

        Vector3 positionPos = seed;
        Vector3 positionNeg = seed;
        r.addPoint(positionPos);
        r.addPoint(positionNeg);

        int panic = 0;

        while (buildPos || buildNeg)
        {
            panic++;
            float directionPos = integrate(positionPos);
            if (major)
            {
                positionPos += new Vector3(Mathf.Sin(directionPos * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(directionPos * Mathf.Deg2Rad) * dStep);
            }
            else
            {
                positionPos += new Vector3(Mathf.Cos(directionPos * Mathf.Deg2Rad) * dStep, 0f, -Mathf.Sin(directionPos * Mathf.Deg2Rad) * dStep);
            }

            float directionNeg = integrate(positionNeg);

            if (major)
            {
                positionNeg -= new Vector3(Mathf.Sin(directionNeg * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(directionNeg * Mathf.Deg2Rad) * dStep);
            }
            else
            {
                positionNeg -= new Vector3(Mathf.Cos(directionNeg * Mathf.Deg2Rad) * dStep, 0f, -Mathf.Sin(directionNeg * Mathf.Deg2Rad) * dStep);
            }

            if (!stopStreamLine(positionPos))
            {
                r.addPoint(positionPos);
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                cube.transform.position = positionPos;
            }
            else
            {
                buildPos = false;
            }

            if (!stopStreamLine(positionNeg))
            {
                r.addPoint(positionNeg);
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                cube.transform.position = positionNeg;
            }
            else
            {
                buildNeg = false;
            }

            if (panic > 10000)
            {
                buildNeg = false;
                buildPos = false;
            }
        }

        return r;
    }

    bool stopStreamLine(Vector3 position)
    {
        if (Vector3.Distance(position, Vector3.zero) > field.totalSize / 2)
        {
            return true;
        }

        return false;
    }

    float integrate(Vector3 point)
    {
        float theta1 = field.sampleTensor(point).theta;
        float theta2 = field.sampleTensor(point + dStep * new Vector3(Mathf.Sin((theta1 / 2f) * (Mathf.PI / 180f)), 0f, Mathf.Cos((theta1 / 2f) * (Mathf.PI / 180f)))).theta;
        float theta3 = field.sampleTensor(point + dStep * new Vector3(Mathf.Sin((theta2 / 2f) * (Mathf.PI / 180f)), 0f, Mathf.Cos((theta2 / 2f) * (Mathf.PI / 180f)))).theta;
        float theta4 = field.sampleTensor(point + dStep * new Vector3(Mathf.Sin((theta3) * (Mathf.PI / 180f)), 0f, Mathf.Cos((theta3) * (Mathf.PI / 180f)))).theta;

        float theta = (theta1 / 6f + theta2 / 3f + theta3 / 3f + theta4 / 6f);
        return theta;
    }

    Vector3 selectSeed(Road r)
    {
        Vector3 seed = Vector3.zero;

        if (r == null)
        {
            return seed;
        }
        else
        {
            Vector3 p = Vector3.zero;
            for (int i = 0; i < 100; i++)
            {
                p = r.points[Random.Range(0, r.points.Count - 1)];

                bool close = false;
                foreach (Road r2 in roads)
                {
                    if (r.major != r2.major)
                    {
                        foreach (Vector3 p2 in r2.points)
                        {
                            if (Vector3.Distance(p, p2) < dSep)
                            {
                                close = true;
                            }
                        }
                    }
                }
                if (!close)
                {
                    Debug.Log("Return p");

                    return p;
                }
            }

            return seed;
        }
    }
}
