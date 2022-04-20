using System.Collections;
using UnityEngine;

public class MeshGeneration : MonoBehaviour
{
    public static MeshGeneration Instance;
    private void Awake() => Instance = this;

    [SerializeField] float thickness;
    [SerializeField] Vector3 startingEulerAngles;
    [SerializeField] Vector3 rotationEulerAngles;
    [SerializeField] Vector3 spiralVelocity;
    [SerializeField] Rigidbody rb;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] CapsuleCollider col;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private float startTime;
    private Quaternion rotation;

    private void Start()
    {
        transform.eulerAngles = startingEulerAngles;

        mesh = new Mesh();
        meshFilter.mesh = mesh;

        CreateMesh();
        UpdateMesh();

        startTime = Time.time;

        rotation = new Quaternion();
        rotation.eulerAngles = rotationEulerAngles;
    }

    public void Generate()
    {
        StartCoroutine(GenerateCoroutine());
    }

    private IEnumerator GenerateCoroutine()
    {
        while (true)
        {
            Rotate(rotation, Vector3.zero);
            Shift(new Vector3(0,(Time.time - startTime) / 10, 0));
            ExtrudeSpiral();
            UpdateMesh();

            yield return new WaitForFixedUpdate();
        }
    }

    private void CreateMesh()
    {
        vertices = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(0,0,0),
            new Vector3(1,0,0)
        };
        triangles = new int[] { 3, 2, 0, 3, 0, 1, 7, 4, 6, 7, 5, 4 };
    }

    private void Shift(Vector3 offset)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i] + offset;
        }
    }

    private void Rotate(Quaternion rotation, Vector3 center)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = rotation * (vertices[i] - center) + center;
        }
    }

    private void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void ExtrudeSpiral()
    {
        int vertexCount = vertices.Length;
        int triangleCount = triangles.Length;
        Vector3[] newVertices = new Vector3[vertexCount + 4];
        int[] newTriangles = new int[triangleCount + 12];

        for (int i = 0; i < vertexCount; i++)
        {
            newVertices[i] = vertices[i];
        }
        for (int i = 0; i < triangleCount; i++)
        {
            newTriangles[i] = triangles[i];
        }

        newVertices[vertexCount] = new Vector3(0, 0, 0);
        newVertices[vertexCount + 1] = new Vector3(1, 0, 0);
        newVertices[vertexCount + 2] = new Vector3(0, 0, thickness);
        newVertices[vertexCount + 3] = new Vector3(1, 0, thickness);

        newTriangles[triangleCount] = vertexCount;
        newTriangles[triangleCount + 1] = vertexCount - 3;
        newTriangles[triangleCount + 2] = vertexCount + 1;
        newTriangles[triangleCount + 3] = vertexCount;
        newTriangles[triangleCount + 4] = vertexCount - 4;
        newTriangles[triangleCount + 5] = vertexCount - 3;

        vertices = newVertices;
        triangles = newTriangles;
    }

    public void StopScraping()
    {
        StopAllCoroutines();
        if (vertices.Length < 50)
        {
            col.center = (vertices[0] + vertices[1]) / 2;
        }
        else
        {
            col.center = (vertices[0] + vertices[49]) / 2;
        }
        col.radius = Vector3.Distance(col.center, vertices[vertices.Length - 5]) - 0.05f;
        col.enabled = true;
        rb.isKinematic = false;
        rb.velocity = spiralVelocity;
    }
}
