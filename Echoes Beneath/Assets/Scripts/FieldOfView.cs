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

    [Header("Mesh Components")]
    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    
    void Start()
    {
        if (darkness != null)
        {
            darkness.SetActive(true);
        }
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }
    private void Update()
    {
        transform.position = playerTransform.position;
    }
    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        List<Vector3> points = new List<Vector3>();
        float angleStep = viewAngle / rayCount;

        points.Add(Vector3.zero);

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.z - viewAngle / 2 + angleStep * i;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewRadius, obstacleMask);

            if (hit.collider != null)
            {
                points.Add(transform.InverseTransformPoint(hit.point));
            }
            else
            {
                points.Add(transform.InverseTransformPoint(transform.position + direction * viewRadius));
            }
        }

        int[] triangles = new int[(rayCount) * 3];
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