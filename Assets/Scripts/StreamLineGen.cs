using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// adapted algorithm from: https://www.researchgate.net/publication/2825680_Creating_Evenly-Spaced_Streamlines_of_Arbitrary_Density
public class StreamLineGen : MonoBehaviour
{
    [Tooltip("min distance between seed points")]
    public float dSep = 10f;
    [Tooltip("min distance between streamlines ")]
    public float dTest = 5f; // generally (according to the paper) half of dSep

    void Start()
    {

    }

    void Update()
    {

    }

    void generateMajorAndMinorRoad(Vector3 seed)
    {
        /* notes:
            - all streamlines must be dSep away from each other.
            - sample points in streamlines must be equal distance from each other and smaller than dsep
            - if a streamline gets close to another streamline, it is discontinued in the current direction
            - streamlines start a central point and move in opposite directions
        */
    }
}
