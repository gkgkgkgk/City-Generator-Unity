using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridField : Field
{
    public float theta;

    public GridField(Vector3 position, float size, float theta, float decay)
    {
        this.position = position;
        this.theta = theta;
        this.decay = decay;
        this.size = size;
    }

    public override Tensor samplePoint(Vector3 p)
    {
        if (Vector3.Distance(position, p) < size)
        {
            float weight = Mathf.Pow(((size - Vector3.Distance(position, p)) / size), decay);
            return new Tensor(position, theta, weight);
        }

        return null;
    }
}
