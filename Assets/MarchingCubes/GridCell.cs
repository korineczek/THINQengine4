using UnityEngine;
using System.Collections;

public class GridCell  {

    public Vector3[] p;
    public bool[] val;

    public GridCell()
    {
        p = new Vector3[8];
        val = new bool[8];
    }

    public GridCell(int x, int y, int z)
    {
        
    }
}
