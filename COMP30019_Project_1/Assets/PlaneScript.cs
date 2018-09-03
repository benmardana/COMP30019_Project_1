using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlaneScript : MonoBehaviour
{
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


        int xs, zs;
        xs = zs = 64;
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
        float[,] heights = PopulateDataArray(xs, 12.0f);

        Vector3[] vertices = new Vector3[(xs + 1) * (zs + 1)];
        for (int i = 0, z = 0; z <= zs; z++)
        {
            for (int x = 0; x <= xs; x++, i++)
            {
                vertices[i] = new Vector3(x, heights[x,z], z);
            }
        }

        return vertices;
    }

    private float[,] PopulateDataArray(int vertices, float roughness)
    {
        int size = vertices+1;
        int max = size - 1;

        float[,] data = new float[size, size];
        float val, rnd;
        float h = roughness;

        int x, y, sideLength, halfSide = 0;

        System.Random r = new System.Random();

        // set the four corner points to inital values
        data[0, 0] = 1f;
        data[max, 0] = 1;
        data[0, max] = 1;
        data[max, max] = 1;


        for (sideLength = max; sideLength >= 2; sideLength /= 2)
        {

            halfSide = sideLength / 2;


            for (x = 0; x < max; x += sideLength)
            {


                for (y = 0; y < max; y += sideLength)
                {



                    float average = getAverage(data[x, y],
                        data[x + sideLength, y], data[x, y + sideLength],
                        data[x + sideLength, y + sideLength]);

                    // add random
                    rnd = ((float)r.NextDouble() * 2.0f * h) - h;
                    val = average + rnd;

                    data[x + halfSide, y + halfSide] = val;
                }

            }

            //diamond values
            for (x = 0; x < max; x += halfSide)
            {


                for (y = (x + halfSide) % sideLength; y < max; y += sideLength)
                {


                    float average = getAverage(data[(x - halfSide + max) % (max), y],
                                        data[(x + halfSide) % (max), y], data[x, (y + halfSide) % (max)],
                                        data[x, (y - halfSide + max) % (max)]);

                    // add random
                    rnd = ((float)r.NextDouble() * 2.0f * h) - h;
                    val = average + rnd;

                    data[x, y] = average;

                    if (x == 0)
                        data[max, y] = val;

                    if (y == 0)
                        data[x, max] = val;
                }

            }
            h /= 2.0f;
        }
        return data;

    }

    // get the average for the four values
    private float getAverage(float a, float b, float c, float d)
    {
        return (a + b + c + d) / 4.0f;
    }


}


