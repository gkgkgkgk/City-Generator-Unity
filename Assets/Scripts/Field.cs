using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Field : ScriptableObject
{
    public float size;
    public float decay;
    public Vector3 position;

    public abstract Tensor samplePoint(Vector3 p);

}
