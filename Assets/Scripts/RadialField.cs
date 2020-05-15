using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialField : Field
{

    public RadialField(Vector3 position, float size, float decay)
    {
        this.position = position;
        this.decay = decay;
        this.size = size;
    }

    public override Tensor samplePoint(Vector3 p)
    {
        float dist = Vector3.Distance(position, p);
        if (dist < size)
        {
            float weight = Mathf.Pow(((size - (dist > size / 2 ? dist : 0)) / size), decay);
            return new Tensor(position, 90f + (Mathf.Rad2Deg * Mathf.Atan2(p.x - position.x, p.z - position.z)), 1.0f);
        }

        return null;
    }
}
