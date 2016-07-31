using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    public int Resolution = 16;
    private float voxelSize, gridSize;
    public GameObject VoxelPrefab;
    private Voxel[] voxels;
    private Material[] voxelMaterials;

    //new mesh data
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles; 

    //neighboring chunks
    public VoxelGrid xNeighbor, yNeighbor, xyNeighbor;

    //neighboring dummy voxels
    private Voxel dummyX, dummyY, dummyT;

    public void Initialize(int resolution, float size)
    {
        this.Resolution = resolution;
        gridSize = size;
        voxelSize = size/Resolution;
        voxels = new Voxel[Resolution*Resolution];
        voxelMaterials =  new Material[voxels.Length];

        dummyX = new Voxel();
        dummyY = new Voxel();
        dummyT = new Voxel();

        for (int i = 0, y = 0; y < Resolution; y++)
        {
            for (int x = 0; x < Resolution; x++, i++)
            {
                CreateVoxel(i, x, y);
            }
        }
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "VoxelGrid Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        Refresh();
    }

    private void CreateVoxel(int i, int x, int y)
    {
        GameObject o = Instantiate(VoxelPrefab) as GameObject;
        
        o.transform.localPosition = new Vector3((x+0.5f)*voxelSize,(y+0.5f)*voxelSize, -0.01f);
        o.transform.parent = this.transform;
        //Debug.Log(o.transform.localPosition);
        o.transform.localScale = Vector3.one*voxelSize*0.1f;
        voxelMaterials[i] = o.GetComponent<MeshRenderer>().material;
        //marching data
        voxels[i] = new Voxel(x,y,voxelSize);
    }


    private void SetVoxelColors()
    {
        for (int i = 0; i < voxels.Length; i++)
        {
            voxelMaterials[i].color = voxels[i].state ? Color.black : Color.white;
        }
    }

    public void Apply(VoxelStencil stencil)
    {
        int xStart = stencil.XStart;
        if (xStart < 0)
        {
            xStart = 0;
        }
        int xEnd = stencil.XEnd;
        if (xEnd >= Resolution)
        {
            xEnd = Resolution - 1;
        }
        int yStart = stencil.YStart;
        if (yStart < 0)
        {
            yStart = 0;
        }
        int yEnd = stencil.YEnd;
        if (yEnd >= Resolution)
        {
            yEnd = Resolution - 1;
        }

        for (int y = yStart; y <= yEnd; y++)
        {
            int i = y * Resolution + xStart;
            for (int x = xStart; x <= xEnd; x++, i++)
            {
                voxels[i].state = stencil.Apply(x, y, voxels[i].state);
            }
        }
        Refresh();
    }

    private void Refresh()
    {
        SetVoxelColors();
        Triangulate();
    }

    private void Triangulate()
    {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();

        if (xNeighbor != null)
        {
            dummyX.BecomeXDummyOf(xNeighbor.voxels[0], gridSize);
        }

        TriangulateCellRows();

        if (yNeighbor != null)
        {
            TriangulateGapRow();
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    private void TriangulateCellRows()
    {
        int cells = Resolution - 1;
        for (int i = 0, y = 0; y < cells; y++, i++)
        {
            for (int x = 0; x < cells; x++, i++)
            {
                TriangulateCell(voxels[i], voxels[i+1], voxels[i+Resolution], voxels[i+Resolution+1]);
            }
            if (xNeighbor != null)
            {
                TriangulateGapCell(i);
            }

        }
    }

    private void TriangulateGapCell(int i)
    {
        Voxel dummySwap = dummyT;
        dummySwap.BecomeXDummyOf(xNeighbor.voxels[i + 1], gridSize);
        dummyT = dummyX;
        dummyX = dummySwap;
        TriangulateCell(voxels[i], dummyT, voxels[i + Resolution], dummyX);
    }

    private void TriangulateGapRow()
    {
        dummyY.BecomeYDummyOf(yNeighbor.voxels[0], gridSize);
        int cells = Resolution - 1;
        int offset = cells * Resolution;

        for (int x = 0; x < cells; x++)
        {
            Voxel dummySwap = dummyT;
            dummySwap.BecomeYDummyOf(yNeighbor.voxels[x + 1], gridSize);
            dummyT = dummyY;
            dummyY = dummySwap;
            TriangulateCell(voxels[x + offset], voxels[x + offset + 1], dummyT, dummyY);
        }

        if (xNeighbor != null)
        {
            dummyT.BecomeXYDummyOf(xyNeighbor.voxels[0], gridSize);
            TriangulateCell(voxels[voxels.Length - 1], dummyX, dummyY, dummyT);
        }
    }

    private void TriangulateCell(Voxel a, Voxel b, Voxel c, Voxel d)
    {
        //bitwise logical operations to add numbers together
        int cellType = 0;
        if (a.state)
        {
            cellType |= 1;
        }
        if (b.state)
        {
            cellType |= 2;
        }
        if (c.state)
        {
            cellType |= 4;
        }
        if (d.state)
        {
            cellType |= 8;
        }

        //marching logic
        switch (cellType)
        {
            //0 filled
            case 0:
                return;
            //1 filled
            case 1:
                AddTriangle(a.position, a.yEdgePosition, a.xEdgePosition);
                break;
            case 2:
                AddTriangle(b.position, a.xEdgePosition, b.yEdgePosition);
                break;
            case 4:
                AddTriangle(c.position, c.xEdgePosition,a.yEdgePosition);
                break;
            case 8:
                AddTriangle(d.position, b.yEdgePosition,c.xEdgePosition);
                break;
            //2 and 4 filled
            case 3:
                AddQuad(a.position,a.yEdgePosition,b.yEdgePosition,b.position);
                break;
            case 5:
                AddQuad(a.position,c.position,c.xEdgePosition,a.xEdgePosition);
                break;
            case 10:
                AddQuad(a.xEdgePosition,c.xEdgePosition,d.position,b.position);
                break;
            case 12:
                AddQuad(a.yEdgePosition,c.position,d.position,b.yEdgePosition);
                break;
            case 15:
                AddQuad(a.position, c.position, d.position, b.position);
                break;
            //3 filled
            case 7:
                AddPentagon(a.position, c.position, c.xEdgePosition, b.yEdgePosition, b.position);
                break;
            case 11:
                AddPentagon(b.position, a.position, a.yEdgePosition, c.xEdgePosition, d.position);
                break;
            case 13:
                AddPentagon(c.position, d.position, b.yEdgePosition, a.xEdgePosition, a.position);
                break;
            case 14:
                AddPentagon(d.position, b.position, a.xEdgePosition, a.yEdgePosition, c.position);
                break;
             //2 filled opposite
            case 6:
                AddTriangle(b.position, a.xEdgePosition, b.yEdgePosition);
                AddTriangle(c.position, c.xEdgePosition, a.yEdgePosition);
                break;
            case 9:
                AddTriangle(a.position, a.yEdgePosition, a.xEdgePosition);
                AddTriangle(d.position, b.yEdgePosition, c.xEdgePosition);
                break;
        }
    }

    private void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex+1);
        triangles.Add(vertexIndex+2);
    }

    private void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    private void AddPentagon(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        vertices.Add(e);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 4);
    }
}
