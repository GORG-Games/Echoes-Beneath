using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject darkness;

    [Header("FOV Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public int rayCount = 50;
    public float smoothingFactor = 0.1f; // Фактор сглаживания

    [Header("Mesh Components")]
    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    private Vector3[] previousVertices;

    void Start()
    {
        if (darkness != null)
        {
            darkness.SetActive(true);
        }
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        previousVertices = new Vector3[rayCount + 1];
    }
    private void Update()
    {
        transform.position = playerTransform.position;
        DrawFieldOfView();
    }
    void LateUpdate()
    {
        //DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        List<Vector3> points = new List<Vector3>();
        float angleStep = viewAngle / rayCount;

        points.Add(Vector3.zero); // Центр Mesh

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.z - viewAngle / 2 + angleStep * i;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewRadius, obstacleMask);

            Vector3 targetPoint = hit.collider != null
                ? transform.InverseTransformPoint(hit.point)
                : transform.InverseTransformPoint(transform.position + direction * viewRadius);

            // Сглаживание между предыдущей и новой вершиной
            Vector3 smoothedPoint = Vector3.Lerp(previousVertices[i], targetPoint, smoothingFactor);

            points.Add(smoothedPoint);
            previousVertices[i] = smoothedPoint; // Обновляем предыдущую вершину
        }

        int[] triangles = new int[rayCount * 3];
        for (int i = 0; i < rayCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        viewMesh.Clear();
        viewMesh.vertices = points.ToArray();
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
}