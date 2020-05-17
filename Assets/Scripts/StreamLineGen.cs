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

        for (int i = 0; i < 100f; i++)
        {
            Debug.Log("new road");
            Road r = generateRoad(seed, major);
            removeInvalidSeeds();
            roads.Add(r);
            seed = findSeed(major);
            major = !major;

            if (seed == Vector3.zero)
            {
                break;
            }
        }
    }


    Road generateRoad(Vector3 seed, bool major)
    {
        Road r = new Road();
        r.major = major;

        bool buildPos = true;
        bool buildNeg = true;

        Vector3 positionPos = seed;
        Vector3 positionNeg = seed;

        int j = 0;

        while (buildPos || buildNeg)
        {
            j++;

            positionPos += integrateStep(positionPos, true, major);

            if (validPoint(positionPos, major))
            {
                r.addPoint(positionPos);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = positionPos;
                sphere.GetComponent<Renderer>().material = major ? majorMat : minorMat;

                if (validSeed(positionPos))
                {
                    if (major)
                    {
                        majorSeeds.Add(positionPos);
                    }
                    else
                    {
                        minorSeeds.Add(positionPos);
                    }
                }
            }
            else
            {
                buildPos = false;
            }

            positionNeg += integrateStep(positionNeg, false, major);

            if (validPoint(positionNeg, major))
            {
                r.addPoint(positionNeg);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = positionNeg;
                sphere.GetComponent<Renderer>().material = major ? majorMat : minorMat;

                if (validSeed(positionNeg))
                {
                    if (major)
                    {
                        majorSeeds.Add(positionNeg);
                    }
                    else
                    {
                        minorSeeds.Add(positionNeg);
                    }
                }
            }
            else
            {
                buildNeg = false;
            }
            if (j > 10000)
            {
                break;
            }

        }

        return r;
    }

    Vector3 findSeed(bool major)
    {
        Vector3 seed = Vector3.zero;

        if (major && majorSeeds.Count > 0)
        {
            int r = Random.Range(0, majorSeeds.Count - 1);
            seed = majorSeeds[r];
            majorSeeds.RemoveAt(r);
        }
        else if (minorSeeds.Count > 0)
        {
            int r = Random.Range(0, minorSeeds.Count - 1);
            seed = minorSeeds[r];
            minorSeeds.RemoveAt(r);
        }

        return seed;
    }

    void removeInvalidSeeds()
    {
        foreach (Road r in roads)
        {
            foreach (Vector3 point in r.points)
            {
                for (int i = majorSeeds.Count - 1; i >= 0; i--)
                {
                    if (Vector3.Distance(majorSeeds[i], point) < dSep)
                    {
                        majorSeeds.RemoveAt(i);
                    }
                }
                for (int i = minorSeeds.Count - 1; i >= 0; i--)
                {
                    if (Vector3.Distance(minorSeeds[i], point) < dSep)
                    {
                        minorSeeds.RemoveAt(i);
                    }
                }
            }
        }
    }

    bool validSeed(Vector3 seed)
    {
        if (Vector3.Distance(seed, Vector3.zero) > field.totalSize / 2f)
        {
            return false;
        }

        foreach (Road r in roads)
        {
            foreach (Vector3 point2 in r.points)
            {
                if (Vector3.Distance(seed, point2) < dSep)
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool validPoint(Vector3 point, bool major)
    {
        if (Vector3.Distance(point, Vector3.zero) > field.totalSize / 2f)
        {
            return false;
        }

        foreach (Road r in roads)
        {
            if (major == r.major)
            {
                foreach (Vector3 point2 in r.points)
                {
                    if (Vector3.Distance(point, point2) < dSep)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }


    Vector3 integrateStep(Vector3 point, bool pos, bool major)
    {

        float dir = normalizeTheta(field.sampleTensor(point).theta, pos, major);
        Vector3 k1 = new Vector3(Mathf.Sin(dir * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(dir * Mathf.Deg2Rad) * dStep);
        float dir2 = normalizeTheta(field.sampleTensor(point + k1 / 2f).theta, pos, major);
        Vector3 k2 = new Vector3(Mathf.Sin(dir2 * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(dir2 * Mathf.Deg2Rad) * dStep);
        float dir3 = normalizeTheta(field.sampleTensor(point + k2 / 2f).theta, pos, major);
        Vector3 k3 = new Vector3(Mathf.Sin(dir3 * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(dir3 * Mathf.Deg2Rad) * dStep);
        float dir4 = normalizeTheta(field.sampleTensor(point + k3).theta, pos, major);
        Vector3 k4 = new Vector3(Mathf.Sin(dir4 * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(dir4 * Mathf.Deg2Rad) * dStep);

        return k1 / 6f + k1 / 3f + k3 / 3f + k4 / 6f;

        //float dir = normalizeTheta(field.sampleTensor(point).theta, pos, major);
    }

    float normalizeTheta(float dir, bool pos, bool major)
    {
        if (major)
        {
            if (!pos)
            {
                // -90 < dir < 90
                return dir + 180f;
            }
            else
            {
                // 90 < dir < 270
                return dir;
            }
        }
        else
        {
            if (!pos)
            {
                // 0 < dir < 180
                return dir - 90f;
            }
            else
            {
                // -180 < dir < 0
                return dir + 90f;
            }
        }
    }

}
