using UnityEngine;
using System.Collections;

public class PlaneScript : MonoBehaviour
{
    public int xSize = 10;
    public int zSize = 10;
    // Use this for initialization
    void Start()
    {
        // Add a MeshFilter component to this entity. This essentially comprises of a
        // mesh definition, which in this example is a collection of vertices, colours 
        // and triangles (groups of three vertices). 
        MeshFilter cubeMesh = this.gameObject.AddComponent<MeshFilter>();
        cubeMesh.mesh = this.CreatePlaneMesh();

        // Add a MeshRenderer component. This component actually renders the mesh that
        // is defined by the MeshFilter component.
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material.shader = Shader.Find("Unlit/VertexColorShader");
    }

    // Method to create a cube mesh with coloured vertices
    Mesh CreatePlaneMesh()
    {
        Mesh m = new Mesh();
        m.name = "Plane";

        // Define the vertices. These are the "points" in 3D space that allow us to
        // construct 3D geometry (by connecting groups of 3 points into triangles).
        Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= zSize; x++, i++)
            {
                vertices[i] = new Vector3(x, 0.0f, z);
            }
        }
        m.vertices = vertices;

        // Define the vertex colours
        Color[] colors = new Color[(xSize + 1) * (zSize + 1)];
        for (int i = 0; i < m.colors.Length; i++) {
            m.colors[i] = Color.green;
        };

        m.colors = colors;

        // Automatically define the triangles based on the number of vertices
        // Task 4: Modify this code to show the interior of the cube instead of the exterior
        // when back-face culling is on.
        int[] triangles = new int[xSize * zSize * 6];
        for (int ti = 0, vi = 0, y = 0; y < zSize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        m.triangles = triangles;

        return m;
    }
}
