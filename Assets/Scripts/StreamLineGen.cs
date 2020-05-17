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
    public List<Vector3> majorSeeds = new List<Vector3>();
    public List<Vector3> minorSeeds = new List<Vector3>();


    public Material minorMat;
    public Material majorMat;

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
        Debug.Log(Mathf.DeltaAngle(-64, 115));
        Debug.Log(Mathf.DeltaAngle(-85, -207));
        buildRoad(Vector3.zero, true);
        buildRoad(Vector3.zero, false);

    }

    void buildRoad(Vector3 seed, bool major)
    {
        bool buildPos = true;
        bool buildNeg = true;
        Vector3 positionPos = seed;
        Vector3 positionNeg = seed;
        float dirPos = field.sampleTensor(seed).theta;
        float dirNeg = field.sampleTensor(seed).theta - 180f;
        int j = 10000;

        while (buildNeg || buildPos)
        {
            j--;

            if (j < 0)
            {
                break;
            }

            dirPos = calculateTheta(positionPos, dirPos, major, true);
            positionPos += new Vector3(Mathf.Sin(dirPos * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(dirPos * Mathf.Deg2Rad) * dStep);

            dirNeg = calculateTheta(positionNeg, dirNeg, major, false);
            positionNeg += new Vector3(Mathf.Sin(dirNeg * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(dirNeg * Mathf.Deg2Rad) * dStep);


            if (validPoint(positionPos))
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = positionPos;
                sphere.AddComponent<RoadSegment>();
                sphere.GetComponent<RoadSegment>().theta = dirPos;
            }
            else
            {
                buildPos = false;
            }

            if (validPoint(positionNeg))
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = positionNeg;
                sphere.AddComponent<RoadSegment>();
                sphere.GetComponent<RoadSegment>().theta = dirNeg;
            }
            else
            {
                buildNeg = false;
            }
        }
    }

    bool validPoint(Vector3 point)
    {
        if (Vector3.Distance(point, Vector3.zero) > field.totalSize / 2f)
        {
            return false;
        }

        return true;
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
            Debug.Log("pt " + previousTheta + "t " + t + "new t: " + (t - 180f));
            t -= 180f;
        }


        return t;
    }

}
