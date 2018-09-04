using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{

    void Start()
    {
        MeshFilter cubeMesh = this.gameObject.AddComponent<MeshFilter>();
        Mesh mesh = cubeMesh.mesh;
        updateMesh(mesh);

        // Add a MeshRenderer component. This component actually renders the mesh that
        // is defined by the MeshFilter component.
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material.shader = Shader.Find("Unlit/PhongShader");
    }

    void Update()
    {
        // pressing space in game mode will generate a new world
        if (Input.GetKeyDown("space"))
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            updateMesh(mesh);
        }
    }

    void updateMesh(Mesh mesh)
    {

        int dimension = 64;

        // transform 2D array into 1D array of vertices
        Vector3[] vertices = new Vector3[(dimension + 1) * (dimension + 1)];
        Color32[] colors = new Color32[vertices.Length];
        for (int i = 0, z = 0; z <= dimension; z++)
        {
            for (int x = 0; x <= dimension; x++, i++)
            {
                vertices[i] = new Vector3(x, 4.0f, z);
                colors[i] = new Color32(64, 164, 223,1);
            }
        }

        mesh.vertices = vertices;
        mesh.colors32 = colors;

        // Turn each quad into two triangles
        // like the below
        // ______
        // |\4  5|
        // |1\   |
        // |  \  |
        // |   \3|
        // |    \|
        // |0___2|

        // ti == triangle index
        // vi == vertices index
        int[] triangles = new int[vertices.Length * 6];
        for (int ti = 0, vi = 0, z = 0; z < dimension; z++, vi++)
        {
            for (int x = 0; x < dimension; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + dimension + 1;
                triangles[ti + 2] = vi + 1;

                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + dimension + 1;
                triangles[ti + 5] = vi + dimension + 2;
            }
        }
        mesh.triangles = triangles;

    }
}
