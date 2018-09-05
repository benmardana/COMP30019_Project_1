using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{

    void Start()
    {}

    // replaces start - called by PlaneScript
    public void setWaterHeight(int size, float height)
    {
        transform.position = Vector3.zero;


        if (this.gameObject.GetComponent<MeshFilter>() == null)
        {
            MeshFilter cubeMesh = this.gameObject.AddComponent<MeshFilter>();
            Mesh mesh = cubeMesh.mesh;
            updateMesh(mesh, size, height);

            MeshCollider meshCollider = this.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }
        else
        {
            MeshFilter cubeMesh = this.gameObject.GetComponent<MeshFilter>();
            Mesh mesh = cubeMesh.mesh;
            updateMesh(mesh, size, height);
            MeshCollider meshCollider = this.gameObject.GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        if (this.gameObject.GetComponent<MeshRenderer>() == null)
        {
            MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
            renderer.material.shader = Shader.Find("WaterPhongShader");
        }
    }

    void updateMesh(Mesh mesh, int dimension, float height)
    {

        // transform 2D array into 1D array of vertices
        Vector3[] vertices = new Vector3[(dimension + 1) * (dimension + 1)];
        Color32[] colors = new Color32[vertices.Length];
        for (int i = 0, z = 0; z <= dimension; z++)
        {
            for (int x = 0; x <= dimension; x++, i++)
            {
                vertices[i] = new Vector3(x, height, z);
                colors[i] = new Color32(64, 164, 223,20);
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
        mesh.RecalculateNormals();

    }
}
