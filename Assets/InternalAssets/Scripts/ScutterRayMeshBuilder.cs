using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScutterRayMeshBuilder : MonoBehaviour
{
    Mesh mesh;
    [SerializeField] MeshCollider mesh_collider;
    [SerializeField] MeshFilter mesh_filter;
    [SerializeField] float base_line;
    [SerializeField] float angle;
    [SerializeField] float distance;

    public void RecalculateMeshFromInspector()
    {
        BuildMesh(base_line, angle, distance);
    }

    public void BuildMesh(float base_line, float angle, float distance)
    {
        if(mesh == null)
            mesh = new Mesh();

        mesh.Clear();
        Vector3 further = new Vector3(0, 0, distance);

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-base_line / 2, 0, 0),
            new Vector3(base_line / 2, 0, 0),
            new Vector3((-base_line / 2) - (Quaternion.Euler(new Vector3(0, angle, 0)) * further).x, 0, distance),
            new Vector3((base_line / 2) + (Quaternion.Euler(new Vector3(0, angle, 0)) * further).x, 0, distance)
        };

        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        mesh_collider.sharedMesh = mesh;
        mesh_filter.sharedMesh = mesh;
    }
}
