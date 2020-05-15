using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tensor
{
    public Vector3 position;
    public float theta;
    public float weight;

    public Tensor(Vector3 position, float theta, float weight)
    {
        this.position = position;
        this.theta = theta;
        this.weight = weight;
    }

    public float getMinorRotation()
    {
        return theta + 90f;
    }

    public float getMajorRotation()
    {
        return theta;
    }
}
