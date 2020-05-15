using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGen : MonoBehaviour
{
    public float totalSize = 100;
    public int numberOfFields = 5;


    public List<Field> fields = new List<Field>();

    [Header("Display Settings")]
    public GameObject majorLine;
    public GameObject minorLine;
    public GameObject fieldCenter;
    public float tensorDisplayDensity = 1f;
    List<Tensor> displayTensors = new List<Tensor>();
    List<GameObject> lines = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        generateRandomFields(numberOfFields);
        sampleDisplayTensors();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void generateRandomFields(float amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (Random.value > 0.5f)
            {
                Vector3 pos = new Vector3((int)Random.Range(-totalSize / 2, totalSize / 2), 0f, (int)Random.Range(-totalSize / 2, totalSize / 2));
                fields.Add(new GridField(pos, Random.Range(10f, 100f), Random.Range(-45f, 45f), 1F / Random.Range(2f, 20f)));
                lines.Add(Instantiate(fieldCenter, pos, Quaternion.identity));
            }
            else
            {
                Vector3 pos = new Vector3((int)Random.Range(-totalSize / 2, totalSize / 2), 0f, (int)Random.Range(-totalSize / 2, totalSize / 2));
                fields.Add(new RadialField(pos, Random.Range(10f, 30f), 1F / Random.Range(2f, 20f)));
                lines.Add(Instantiate(fieldCenter, pos, Quaternion.identity));
            }
        }
    }

    void sampleDisplayTensors()
    {
        for (float i = -totalSize / 2; i < totalSize / 2; i += tensorDisplayDensity)
        {
            for (float j = -totalSize / 2; j < totalSize / 2; j += tensorDisplayDensity)
            {
                Tensor t = sampleTensor(new Vector3(i, 0f, j));
                displayTensors.Add(t);
                GameObject minor = Instantiate(minorLine, new Vector3(i, 0f, j), Quaternion.Euler(0f, t.getMinorRotation(), 0f));
                GameObject major = Instantiate(majorLine, new Vector3(i, 0f, j), Quaternion.Euler(0f, t.getMajorRotation(), 0f));
                lines.Add(minor);
                lines.Add(major);
            }
        }
    }

    Tensor sampleTensor(Vector3 position)
    {
        float theta = 0f;
        float weight = 0f;

        foreach (Field field in fields)
        {
            Tensor t = field.samplePoint(position);
            if (t != null)
            {
                theta += (t.theta * t.weight);
                weight += t.weight;
            }
        }

        theta = weight == 0 ? 0f : theta / weight;

        return new Tensor(position, theta, weight);
    }

    public void drawTensors()
    {
        foreach (GameObject line in lines)
        {
            line.SetActive(true);
        }
    }

    public void hideTensors()
    {
        foreach (GameObject line in lines)
        {
            line.SetActive(false);
        }
    }

    public void drawFields()
    {

    }

    public void hideFields()
    {

    }
}
