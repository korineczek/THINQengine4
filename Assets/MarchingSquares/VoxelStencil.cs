﻿using UnityEngine;
using System.Collections;

public class VoxelStencil
{

    private bool fillType;
    private int centerX, centerY, radius;

    public int XStart
    {
        get
        {
            return centerX - radius;
        }
    }

    public int XEnd
    {
        get
        {
            return centerX + radius;
        }
    }

    public int YStart
    {
        get
        {
            return centerY - radius;
        }
    }

    public int YEnd
    {
        get
        {
            return centerY + radius;
        }
    }

    public void Intitialize(bool fillType, int radius)
    {
        this.fillType = fillType;
        this.radius = radius;
    }

    public void SetCenter(int x, int y)
    {
        centerX = x;
        centerY = y;
    }

    public bool Apply(int x, int y, bool voxel)
    {
        return fillType;
    }

}
