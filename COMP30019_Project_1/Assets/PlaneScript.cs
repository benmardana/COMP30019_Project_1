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

        public int MiddleIndex
        {
        	get 
        	{
            	return ((d - a) / 2) + a;
        	}
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

        public int MiddleIndex
        {
        	get 
        	{
            	return ((c - b) / 2) + b;
        	}
        }
    }

    private HashSet<Square> squaresSet = new HashSet<Square>();
    private HashSet<Diamond> diamondsSet = new HashSet<Diamond>();

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
        
        Vector3[] vertices = new Vector3[(xs + 1) * (zs + 1)];
        for (int i = 0, z = 0; z <= zs; z++)
        {
            for (float x = 0; x <= xs; x++, i++)
            {
                vertices[i] = new Vector3(x, 0.0f, z);
            }
        }

        // create initial 'square'
        int a = 0;
        int d = vertices.Length - 1;
        int b = a + xs;
        int c = d - xs;
        Square initSquare = new Square(a, b, c, d);

        vertices[initSquare.a].y = 2.0f;
        vertices[initSquare.b].y = 1.5f;
        vertices[initSquare.c].y = 1.0f;
        vertices[initSquare.d].y = 0.5f;

        // add to 'squares'
        squaresSet.Add(initSquare);
        Debug.Log(squaresSet.Count);
        Debug.Log(diamondsSet.Count);

        // while squares || diamonds
        while ((squaresSet.Count > 0) || (diamondsSet.Count > 0))
        {
            // make lists of diamonds and squares to be removed after iteration
            List<Square> squaresGC = new List<Square>();
            List<Diamond> diamondsGC = new List<Diamond>();
            // squares first
            foreach (Square square in squaresSet)
            {
                // get average
                Vector3 A = vertices[square.a];
                Vector3 B = vertices[square.b];
                Vector3 C = vertices[square.c];
                Vector3 D = vertices[square.d];
                float sqAvg = (A.y + B.y + C.y + D.y) / 4;

                // add modifier
                float randAvg = sqAvg + Random.Range(0.0f, 5.0f);

                // set middle value of square
                vertices[square.MiddleIndex].y = randAvg;
                Debug.Log("Setting square at ");
                Debug.Log(square.MiddleIndex);
                Debug.Log("with value ");
                Debug.Log(randAvg);

                // new diamonds
                Diamond[] newDiamonds = new Diamond[4];
                // if on the first square, use wraparound values
                if (square.a == 0) 
                {
                	// up
                    newDiamonds[0] = new Diamond(square.c, square.MiddleIndex, square.MiddleIndex, square.d);
                	// down
                	newDiamonds[1] = new Diamond(square.a, square.MiddleIndex, square.MiddleIndex, square.b);
                	// left
                	newDiamonds[2] = new Diamond(square.MiddleIndex, square.a, square.c, square.MiddleIndex);
                	// right
                    newDiamonds[3] = new Diamond(square.MiddleIndex, square.b, square.d, square.MiddleIndex);
                }
                else
                {
                	// up
                    newDiamonds[0] = new Diamond(square.c, square.MiddleIndex, square.MiddleIndex + ((square.b - square.a)*xs), square.d);
                    // down
                    newDiamonds[1] = new Diamond(square.a, square.MiddleIndex - ((square.b - square.a)*xs), square.MiddleIndex, square.b);
                    // left
                    newDiamonds[2] = new Diamond(square.MiddleIndex - (square.b - square.a), square.a, square.c, square.MiddleIndex);
                    // right
                    newDiamonds[3] = new Diamond(square.MiddleIndex, square.b, square.d, square.MiddleIndex + (square.b - square.a));
                }

                // if possible diamond values are not already set
                foreach (Diamond diamond in newDiamonds) 
                {
                    // if not set
                    Debug.Log("adding diamond");
                	Debug.Log(diamond.MiddleIndex);
                    if (vertices[diamond.MiddleIndex].y == 0.0f)
                    {
                        // add to diamonds array
                        diamondsSet.Add(diamond);
                    }
                }

                // add square to squaresGC
                squaresGC.Add(square);
            }

            // then diamonds
            foreach (Diamond diamond in diamondsSet)
            {
                // get average
                Vector3 A = vertices[diamond.a];
                Vector3 B = vertices[diamond.b];
                Vector3 C = vertices[diamond.c];
                Vector3 D = vertices[diamond.d];
                float sqAvg = (A.y + B.y + C.y + D.y) / 4;

                // add modifier
                float randAvg = sqAvg + Random.Range(0.0f, 5.0f);

                // set middle value of diamond
                vertices[diamond.MiddleIndex].y = randAvg;
                Debug.Log("Setting diamond at ");
                Debug.Log(diamond.MiddleIndex);
                Debug.Log("with value ");
                Debug.Log(randAvg);
                
                // new squares
                Square[] newSquares = new Square[4];

                // topL
                newSquares[0] = new Square(diamond.a, diamond.MiddleIndex, diamond.a + ((diamond.c - diamond.b)/2), diamond.c);
                // topR
                newSquares[1] = new Square(diamond.MiddleIndex, diamond.d, diamond.c, diamond.d + ((diamond.c - diamond.b)/2));
                // botL
                newSquares[2] = new Square(diamond.a - ((diamond.c - diamond.b)/2), diamond.b, diamond.a, diamond.MiddleIndex);
                // botR
                newSquares[3] = new Square(diamond.b, diamond.d - ((diamond.c - diamond.b)/2), diamond.MiddleIndex, diamond.d);


                // if possible diamond values are not already set
                foreach (Square square in newSquares) 
                {
                    // if not set
                    if (vertices[square.MiddleIndex].y == 0.0f)
                    {
                        // add to diamonds hashset
                        squaresSet.Add(square);
                    }
                }

                // add square to squaresGC
                diamondsGC.Add(diamond);
            }

            //remove squaresGC and diamondsGC from squaresSet and diamondsSet
            foreach (Square square in squaresGC)
            {
                squaresSet.Remove(square);
            }

            foreach (Diamond diamond in diamondsGC)
            {
                diamondsSet.Remove(diamond);
            }
        }

    return vertices;
    }
}
