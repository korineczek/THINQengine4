using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MarchingCubes : MonoBehaviour
{
    private int cubeIndex;
    private Vector3[] vertList = new Vector3[12];
    private GridCell cell;
    private Vector3 chunkOffset;

    //debug shit
    public Transform DebugCubeTrue;
    public Transform DebugCubeFalse;

    //mesh info
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles; 

    public void Awake()
    {
        chunkOffset = this.transform.position;
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }


    public void MarchCell(GridCell cell)
    {
        //determine cube index for table lookup

        cubeIndex = 0;
        if (cell.val[0]) { cubeIndex |= 1;   }
        if (cell.val[1]) { cubeIndex |= 2;   }
        if (cell.val[2]) { cubeIndex |= 4;   }
        if (cell.val[3]) { cubeIndex |= 8;   }
        if (cell.val[4]) { cubeIndex |= 16;  }
        if (cell.val[5]) { cubeIndex |= 32;  }
        if (cell.val[6]) { cubeIndex |= 64;  }
        if (cell.val[7]) { cubeIndex |= 128; }

        //cube out of surface
        if (LookupTables.EdgeTable[cubeIndex] == 0)
        {
            return;
        }
        //find vertices that intersect the cube
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 1))
        {
            vertList[0] = VertexInterp(cell.p[0], cell.p[1], cell.val[0], cell.val[1]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 2))
        {
            vertList[1] = VertexInterp(cell.p[1], cell.p[2], cell.val[1], cell.val[2]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 4))
        {
            vertList[2] = VertexInterp(cell.p[2], cell.p[3], cell.val[2], cell.val[3]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 8))
        {
            vertList[3] = VertexInterp(cell.p[3], cell.p[0], cell.val[3], cell.val[0]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 16))
        {
            vertList[4] = VertexInterp(cell.p[4], cell.p[5], cell.val[4], cell.val[5]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 32))
        {
            vertList[5] = VertexInterp(cell.p[5], cell.p[6], cell.val[5], cell.val[6]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 64))
        {
            vertList[6] = VertexInterp(cell.p[6], cell.p[7], cell.val[6], cell.val[7]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 128))
        {
            vertList[7] = VertexInterp(cell.p[7], cell.p[4], cell.val[7], cell.val[4]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 256))
        {
            vertList[8] = VertexInterp(cell.p[0], cell.p[4], cell.val[0], cell.val[4]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 512))
        {
            vertList[9] = VertexInterp(cell.p[1], cell.p[5], cell.val[1], cell.val[5]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 1024))
        {
            vertList[10] = VertexInterp(cell.p[2], cell.p[6], cell.val[2], cell.val[6]);
        }
        if (IsBitSet(LookupTables.EdgeTable[cubeIndex], 2048))
        {
            vertList[11] = VertexInterp(cell.p[3], cell.p[7], cell.val[3], cell.val[7]);
        }

        //make triangle faces
        for (int i = 0; LookupTables.TriTable[cubeIndex, i] != -1; i += 3)
        {
            int index = vertices.Count;
            vertices.Add(vertList[LookupTables.TriTable[cubeIndex, i]] + chunkOffset);
            vertices.Add(vertList[LookupTables.TriTable[cubeIndex, i + 2]] + chunkOffset);
            vertices.Add(vertList[LookupTables.TriTable[cubeIndex, i + 1]] + chunkOffset);
            triangles.Add(index);
            triangles.Add(index + 1);
            triangles.Add(index + 2);
        }

    }

    /// <summary>
    /// compare numbers bitwise to see if a specific edge needs cutting
    /// </summary>
    /// <param name="b"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    bool IsBitSet(int b, int pos)
    {
        return ((b & pos) == pos);
    }

    /// <summary>
    /// interpolate vectors to find intersections or points on edges
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    private Vector3 VertexInterp(Vector3 p1, Vector3 p2, bool v1, bool v2)
    {
        return (p1 + p2)/2;
    }

    public Mesh CreateMesh(Mesh mesh)
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }

        mesh.name = "TestMesh";
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles,0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public void ClearLists()
    {
        //clear previous shit
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }
}
