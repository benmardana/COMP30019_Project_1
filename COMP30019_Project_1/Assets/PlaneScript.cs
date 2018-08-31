using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaneScript : MonoBehaviour
{
    // for mesh size
    public int xSize = 10;
    public int zSize = 10;
    public int granularity = 1; // multiplier for grid density, higher int means more squares

    // for diamond-square algorithm
    public float noise;

    public struct Square
    // c------------d
    // |            |
    // |            |
    // |            |
    // |            |
    // |            |
    // a------------b
    {
        public readonly int a, b, c, d;

        public Square(int a, int b, int c, int d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public int Middle()
        {
            return ((d - a) / 2) + a;
        }
    }

    public struct Diamond
    //      c
    //      /\
    //     /  \
    //    /    \
    //   /      \
    //  /        \
    // a          d
    //  \        /
    //   \      /
    //    \    /
    //     \  /
    //      \/
    //      b
    {
        public readonly int a, b, c, d;

        public Diamond(int a, int b, int c, int d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public int Middle()
        {
            return (d - a) / 2 + a;
        }
    }

    private HashSet<Square> squares = new HashSet<Square>();
    private HashSet<Diamond> diamonds = new HashSet<Diamond>();

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

        // modify grid size with granularity modifier
        int xs = xSize * granularity;
        int zs = zSize * granularity;

        m.vertices = GenerateVertexMap(xs, zs);

        // vertex colours 
        // use vertex "height" for map implementation when ds algo complete
        Color32[] colors = new Color32[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
        {
            colors[i] = new Color32(193, 66, 66, 125);
        };

        m.colors32 = colors;

        // Turn each quad into two triangles
        // like the below image
        // ______
        // |\4  5|
        // |1\   |
        // |  \  |
        // |   \3|
        // |    \|
        // |0___2|

        // ti == triangle index
        // vi == vertices index
        int[] triangles = new int[xs * zs * 6];
        for (int ti = 0, vi = 0, z = 0; z < zs; z++, vi++)
        {
            for (int x = 0; x < xs; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + xs + 1;
                triangles[ti + 2] = vi + 1;

                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + xs + 1;
                triangles[ti + 5] = vi + xs + 2;
            }
        }

        m.triangles = triangles;

        return m;
    }

    Vector3[] GenerateVertexMap(int xs, int zs)
    {
        // Diamond Square Algorithm
        // TODO - implement diamond square algorithm on flat vertex array
        
        Vector3[] flatvertices = new Vector3[(xs + 1) * (zs + 1)];
        for (int i = 0, z = 0; z <= zs; z++)
        {
            for (float x = 0; x <= xs; x++, i++)
            {
                flatvertices[i] = new Vector3(x, 0.0f, z);
            }
        }
        return flatvertices;
    }

}

