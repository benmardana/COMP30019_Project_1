﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlaneScript : MonoBehaviour
{
    public int sideSize = 64;
    float roughness = 15.0f;
    float edgeHeight = 5.0f;
    float snowPerc = 0.9f;
    float rockPerc = 0.7f;
    float grassPerc = 0.4f;
    float sandPerc = 0.3f;

    GameObject referenceObject;
    WaterScript referenceScript;

    void Start()
    {

        MeshFilter cubeMesh = this.gameObject.AddComponent<MeshFilter>();
        Mesh mesh = cubeMesh.mesh;
        updateMesh(mesh);

        MeshCollider meshCollider = this.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        // Add a MeshRenderer component. This component actually renders the mesh that
        // is defined by the MeshFilter component.
        MeshRenderer rend = this.gameObject.AddComponent<MeshRenderer>();
        rend.material.shader = Shader.Find("PhongShader");
        
        transform.position = Vector3.zero;
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
        // copy the existing attributes to set
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = mesh.colors32;
        int[] triangles = mesh.triangles;

        // set values
        vertices = GenerateVertexMap(sideSize, roughness);
        colors = ApplyColors(vertices);
        triangles = GenerateTriangles(vertices);

        // apply back
        mesh.vertices = vertices;
        mesh.colors32 = colors;
        mesh.triangles = triangles;

        // set normals
        mesh.RecalculateNormals();


    }

    Vector3[] GenerateVertexMap(int sideSize, float roughness)
    {
        // generate 2D height array using diamond square algorithm
        float[,] heights = GenerateHeightMap(sideSize, roughness);

        // transform 2D array into 1D array of vertices
        Vector3[] vertices = new Vector3[(sideSize + 1) * (sideSize + 1)];
        for (int i = 0, z = 0; z <= sideSize; z++)
        {
            for (int x = 0; x <= sideSize; x++, i++)
            {
                vertices[i] = new Vector3(x, heights[x, z], z);
            }
        }
        return vertices;
    }

    Color32[] ApplyColors(Vector3[] vertices)
    {

        // set color bands as percentages of total height
        Color32[] colors = new Color32[vertices.Length];
        float[] maxAndMin = getMaxAndMin(vertices);
        float max = maxAndMin[0];
        float min = maxAndMin[1];
        float diff = max - min;

        float snowHeight = diff * snowPerc + min;
        float rockHeight = diff * rockPerc + min;
        float grassHeight = diff * grassPerc + min;
        float sandHeight = diff * sandPerc + min;
        float waterHeight = grassHeight - ((grassHeight - sandHeight) * 0.6f);

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y <= sandHeight) 
            {
                // brown
                colors[i] = new Color32(92, 67, 53, 255);
            }
            else if (vertices[i].y <= grassHeight) 
            {
                // sandy
                colors[i] = new Color32(232, 196, 128, 255);
            }
            else if (vertices[i].y <= rockHeight) 
            {
                // green grass
                colors[i] = new Color32(37, 136, 42, 255);
            }
            else if (vertices[i].y <= snowHeight) 
            {
                // rock grey
                colors[i] = new Color32(153, 153, 152, 255);
            }
            else 
            {
                // snow white
                colors[i] = new Color32(255, 255, 255, 255);
            }
        };

        // update the water now
        referenceObject = GameObject.Find("Water");
        referenceScript = referenceObject.GetComponent<WaterScript>();
        referenceScript.setWaterHeight(sideSize, waterHeight);

        return colors;
    }

    int[] GenerateTriangles(Vector3[] vertices)
    {
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
        for (int ti = 0, vi = 0, z = 0; z < sideSize; z++, vi++)
        {
            for (int x = 0; x < sideSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + sideSize + 1;
                triangles[ti + 2] = vi + 1;

                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + sideSize + 1;
                triangles[ti + 5] = vi + sideSize + 2;
            }
        }

        return triangles;
    } 

    private float[,] GenerateHeightMap(int vertices, float roughness)
    {
        int size = vertices + 1;

        float[,] heights = new float[size, size];
        float val, random;

        int x, z, sideLength, halfSide = 0;

        System.Random r = new System.Random();

        // set the four corner points to inital values
        heights[0, 0] = edgeHeight;
        heights[vertices, 0] = edgeHeight;
        heights[0, vertices] = edgeHeight;
        heights[vertices, vertices] = edgeHeight;

        for (sideLength = vertices; sideLength >= 2; sideLength /= 2)
        {
            halfSide = sideLength / 2;

            // squares
            for (x = 0; x < vertices; x += sideLength)
            {
                for (z = 0; z < vertices; z += sideLength)
                {
                    float average = (heights[x, z] +
                                    heights[x + sideLength, z] +
                                    heights[x, z + sideLength] +
                                    heights[x + sideLength, z + sideLength]) / 4;

                    // Random value is any value in the range from -roughness to +roughness (i.e. -15 -> 15)
                    random = ((float)r.NextDouble() * 2.0f * roughness) - roughness;
                    val = average + random;

                    // square edges
                    if (x == 0 || z == 0)
                    {
                        val = edgeHeight;
                    }

                    if (x == 0)
                    {
                    	heights[vertices, z] = val;
                    }
                    if (z == 0)
                    {
                    	heights[x, vertices] = val;
                    }

                    heights[x + halfSide, z + halfSide] = val;
                }

            }

            // diamonds
            for (x = 0; x < vertices; x += halfSide)
            {
                for (z = (x + halfSide) % sideLength; z < vertices; z += sideLength)
                {
                    float average = (heights[(x - halfSide + vertices) % (vertices), z] +
                                    heights[(x + halfSide) % (vertices), z] +
                                    heights[x, (z + halfSide) % (vertices)] +
                                    heights[x, (z - halfSide + vertices) % (vertices)]) / 4;

                    random = ((float)r.NextDouble() * 2.0f * roughness) - roughness;
                    val = average + random;

                    // square edges
                    if (x == 0 || z == 0)
                    {
                        val = edgeHeight;
                    }

                    if (x == 0)
                    {
                    	heights[vertices, z] = val;
                    }
                    if (z == 0)
                    {
                    	heights[x, vertices] = val;
                    }

                    heights[x, z] = val;
                }

            }
            // 
            roughness /= 1.6f;
        }
        return heights;
    }

    float[] getMaxAndMin(Vector3[] array)
    {
        float max = array[0].y;
        float min = array[0].y;
        foreach (Vector3 vertex in array) 
        {
            if (vertex.y < min) 
            {
                min = vertex.y;
            }
            if (vertex.y > max) 
            {
                max = vertex.y;
            }
        }
        return new float[]{max, min};
    }

    public int getLimit()
    {
        return sideSize;
    }
}


