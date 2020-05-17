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
        generateRoad(Vector3.zero, major);

        for (int i = 0; i < 10f; i++)
        {
            major = !major;
            Vector3 seed = selectSeed(major);
            if (seed != Vector3.zero)
            {
                generateRoad(seed, major);
            }
            else
            {
                break;
            }
        }
    }

    Road generateRoad(Vector3 seed, bool major)
    {
        Road r = new Road();
        roads.Add(r);
        bool buildPos = true;
        bool buildNeg = true;
        int j = 0;

        Vector3 positionPos = Vector3.zero;
        Vector3 positionNeg = Vector3.zero;

        while (j < 10000 && (buildNeg || buildPos))
        {
            j++;


            if (buildPos)
            {
                positionPos += integrateStep(positionPos, true, major);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = positionPos;

                if (validPoint(positionPos))
                {
                    r.addPoint(positionPos);
                    if (major)
                    {
                        majorSeeds.Add(positionPos);

                    }
                    else
                    {
                        minorSeeds.Add(positionPos);

                    }
                }
                else
                {
                    buildPos = false;
                }
            }

            if (buildNeg)
            {
                positionNeg += integrateStep(positionNeg, false, major);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = positionNeg;

                if (validPoint(positionNeg))
                {
                    r.addPoint(positionNeg);
                    if (major)
                    {
                        majorSeeds.Add(positionNeg);

                    }
                    else
                    {
                        minorSeeds.Add(positionNeg);

                    }
                }
                else
                {
                    buildNeg = false;
                }
            }

        }

        return r;
    }

    Vector3 selectSeed(bool major)
    {
        Vector3 seed = Vector3.zero;

        if (potentialSeeds(major).Count > 0)
        {
            int count = potentialSeeds(major).Count;
            seed = potentialSeeds(major)[count - 1];
            potentialSeeds(major).RemoveAt(count - 1);
            if (validSeed(major, seed))
            {
                return seed;
            }
        }

        seed = Vector3.zero;
        // try getting a new seed 100 times before giving up
        for (int i = 0; i < 100; i++)
        {
            seed = new Vector3(Random.Range(-field.totalSize / 2, field.totalSize / 2), 0f, Random.Range(-field.totalSize / 2, field.totalSize / 2));
            if (validSeed(major, seed))
            {
                return seed;
            }
        }

        return seed;
    }

    List<Vector3> potentialSeeds(bool major)
    {
        return major ? majorSeeds : minorSeeds;
    }

    bool validSeed(bool major, Vector3 seed)
    {
        // there is DEFINITELY a more efficient way to do this.
        bool valid = true;
        foreach (Road road in roads)
        {
            foreach (Vector3 point in road.points)
            {
                if (Vector3.Distance(point, seed) < dSep)
                {
                    valid = false;
                }
            }
        }

        return valid;
    }

    bool validPoint(Vector3 p)
    {

        if (Vector3.Distance(p, Vector3.zero) > field.totalSize / 2f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool stopStreamLine(Vector3 position)
    {
        return false;
    }

    float integrate(Vector3 point)
    {
        return field.sampleTensor(point).theta;
    }

    Vector3 integrateStep(Vector3 point, bool pos, bool major)
    {
        float dir = integrate(point);

        if (pos)
        {
            if (major)
            {
                return new Vector3(Mathf.Sin(dir * Mathf.Deg2Rad) * dStep, 0f, Mathf.Cos(dir * Mathf.Deg2Rad) * dStep);
            }
            else
            {
                return new Vector3(Mathf.Cos(dir * Mathf.Deg2Rad) * dStep, 0f, -Mathf.Sin(dir * Mathf.Deg2Rad) * dStep);
            }
        }
        else
        {
            if (major)
            {
                return new Vector3(-Mathf.Sin(dir * Mathf.Deg2Rad) * dStep, 0f, -Mathf.Cos(dir * Mathf.Deg2Rad) * dStep);
            }
            else
            {
                return new Vector3(-Mathf.Cos(dir * Mathf.Deg2Rad) * dStep, 0f, Mathf.Sin(dir * Mathf.Deg2Rad) * dStep);
            }
        }
    }
}
