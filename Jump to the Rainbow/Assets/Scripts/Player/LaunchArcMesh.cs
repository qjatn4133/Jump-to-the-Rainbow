using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class LaunchArcMesh : MonoBehaviour {

    public PlayerController controller;
    public GameObject landingPointMesh;
    Mesh mesh;

    public float meshWidth;
    public float angle;
    public int resolution = 0;
    public float velocityAdjust;
    float velocity;

    public float g; // force of gravity on the Y axis
    float radianAngle;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void OnValidate()
    {
        // check that mesh is not null and that the game is playing
        if(mesh != null && Application.isPlaying)
        {
            MakeArcMesh(CalculateArcArray());
        }
    }
    
    /*
    // use this for initialization
    private void Start()
    {
        MakeArcMesh(CalculateArcArray());
    }
    */

    // Slider의 Value를 Velocity에 적용
    private void Update()
    {
        velocity = (controller.velocity * velocityAdjust);
        MakeArcMesh(CalculateArcArray());

    }

    private void OnDisable()
    {
        mesh.Clear();
    }

    /*
    public IEnumerator StartMakeArcMesh()
    {
        while(!controller.inputUIController.DragEnd)
        {
            velocity = (controller.velocity * velocityAdjust);
            MakeArcMesh(CalculateArcArray());

            yield return null;
        }
    }
    */

    void MakeArcMesh(Vector3[] arcVerts)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 6 * 2];

        for (int i = 0; i <= resolution; i++)
        {
            // set vertices
            vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVerts[i].y, arcVerts[i].z);
            vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVerts[i].y, arcVerts[i].z);

            // set triangles
            if (i != resolution)
            {
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = i * 2 + 1;
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i + 1) * 2;
                triangles[i * 12 + 5] = (i + 1) * 2 + 1;

                triangles[i * 12 + 6] = i * 2;
                triangles[i * 12 + 7] = triangles[i * 12 + 10] = (i + 1) * 2;
                triangles[i * 12 + 8] = triangles[i * 12 + 9] = i * 2 + 1;
                triangles[i * 12 + 11] = (i + 1) * 2 + 1;
            }




            mesh.vertices = vertices;
            mesh.triangles = triangles;
        }
    }

    void MakeTriggerArray(Vector3[] pointVerts)
    {
        GameObject[] landingPoints = new GameObject[resolution + 1];

        for (int i = 0; i <= resolution; i++)
        {
            Destroy(landingPoints[i]);
        }

        for (int i = 0; i <= resolution; i++)
        {
            GameObject landingPoint = Instantiate(landingPointMesh, pointVerts[i], Quaternion.identity);

            landingPoints[i] = landingPoint;
        }
    }

    // create an array of Vector3 positions for arc
    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    // calculate height and distance of each vertex
    public Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float z = t * maxDistance;
        float y = z * Mathf.Tan(radianAngle) - ((g * z * z) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));

        return new Vector3(0, y, z);
    }
}
