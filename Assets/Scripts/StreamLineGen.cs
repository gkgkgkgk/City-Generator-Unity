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
        bool major = true;
        Vector3 seed = Vector3.zero;
        for (int i = 0; i < field.totalSize * 10f; i++)
        {
            Road r = buildRoad(seed, major);
            r.id = i;
            roads.Add(r);
            major = !major;
            seed = newSeed(major);

            if (Vector3.zero == seed)
            {
                Debug.Log("zero seed!");
                break;
            }
        }
    }

    Road buildRoad(Vector3 seed, bool major)
    {
        Road r = new Road();
        r.major = major;
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


            if (validPoint(positionPos, r))
            {
                r.addPoint(positionPos);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = positionPos;
                sphere.AddComponent<RoadSegment>();
                sphere.GetComponent<RoadSegment>().theta = dirPos;
                sphere.GetComponent<RoadSegment>().id = dirPos;
            }
            else
            {
                buildPos = false;
            }

            if (validPoint(positionNeg, r))
            {
                r.addPoint(positionNeg);
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

        return r;
    }

    bool validPoint(Vector3 point, Road r)
    {
        // check for out of bounds
        if (Vector3.Distance(point, Vector3.zero) > field.totalSize / 2f)
        {
            Debug.Log("Stopped out of bounds");
            return false;
        }

        // check intersection with similar road
        foreach (Road r2 in roads)
        {
            if (r2.major == r.major)
            {
                foreach (Vector3 point2 in r2.points)
                {
                    if (Vector3.Distance(point, point2) < dSep)
                    {
                        Debug.Log("Stopped intersection " + point + " " + point2);
                        return false;
                    }
                }
            }
        }

        // check for circle
        foreach (Vector3 point2 in r.points)
        {
            if (Vector3.Distance(point, point2) < dStep / 2f)
            {
                Debug.Log("Stopped circle");
                return false;
            }
        }

        return true;
    }

    Vector3 newSeed(bool major)
    {
        return new Vector3(Random.Range(-field.totalSize / 2f, field.totalSize / 2f), 0f, Random.Range(-field.totalSize / 2f, field.totalSize / 2f));


        return Vector3.zero;
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

/* TODO: 
- remove/join Dangling roads
- remove small roads
- detect intersections
- convert roads to a layer of tensor field
*/
