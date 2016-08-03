﻿using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour
{


    private int size = 16;
    public Transform DebugCube;
    private Vector3 ChunkPos;

    private bool[,,] chunkVals = new bool[16,16,16];
    private GridCell currentCell;
    private MarchingCubes marchingRenderer;

    private int xPos;
    private int yPos;
    private int zPos;

	// Use this for initialization
	void Start ()
	{
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.y;
        zPos = (int)transform.position.z;
	    marchingRenderer = this.GetComponent<MarchingCubes>();
        currentCell = new GridCell();

	    ChunkPos = this.transform.position;
	    for (int x = 0; x < size; x++)
	    {
	        for (int y = 0; y < size; y++)
	        {
                for (int z = 15; z >= 0; z--)
	            {
	                Instantiate(DebugCube, new Vector3(x, y, z), Quaternion.identity);
	            }
	        }
	    }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                hitInfo.transform.GetComponent<MeshRenderer>().material.color = new Color(1,0,0);
                chunkVals[(int)hitInfo.transform.position.x - xPos,(int)hitInfo.transform.position.y - xPos,(int)hitInfo.transform.position.z - xPos] = true;
                MarchChunk();
            }
        }
	}

    /// <summary>
    /// try to march the entire chunk
    /// </summary>
    private void MarchChunk()
    {
        marchingRenderer.ClearLists();
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.y;
        zPos = (int)transform.position.z;
        for (int x = 0; x < size - 1; x++)
        {
            for (int y = 0; y < size - 1; y++)
            {
                for (int z = 15; z > 0; z--)
                {
                    //add vertex values to the current step
                    currentCell.val[0] = chunkVals[x    , y    , z    ];
                    currentCell.val[1] = chunkVals[x + 1, y    , z    ];
                    currentCell.val[2] = chunkVals[x + 1, y    , z - 1];
                    currentCell.val[3] = chunkVals[x    , y    , z - 1];
                    currentCell.val[4] = chunkVals[x    , y + 1, z    ];
                    currentCell.val[5] = chunkVals[x + 1, y + 1, z    ];
                    currentCell.val[6] = chunkVals[x + 1, y + 1, z - 1];
                    currentCell.val[7] = chunkVals[x    , y + 1, z - 1];
                    //add vertex positions to the current step
                    currentCell.p[0] = new Vector3(x     - xPos, y     - yPos, z     - zPos);
                    currentCell.p[1] = new Vector3(x + 1 - xPos, y     - yPos, z     - zPos);
                    currentCell.p[2] = new Vector3(x + 1 - xPos, y     - yPos, z - 1 - zPos);
                    currentCell.p[3] = new Vector3(x     - xPos, y     - yPos, z - 1 - zPos);
                    currentCell.p[4] = new Vector3(x     - xPos, y + 1 - yPos, z     - zPos);
                    currentCell.p[5] = new Vector3(x + 1 - xPos, y + 1 - yPos, z     - zPos);
                    currentCell.p[6] = new Vector3(x + 1 - xPos, y + 1 - yPos, z - 1 - zPos);
                    currentCell.p[7] = new Vector3(x     - xPos, y + 1 - yPos, z - 1 - zPos);   
        
                    //march current cell
                    marchingRenderer.MarchCell(currentCell);
                    marchingRenderer.CreateMesh(this.GetComponent<MeshFilter>().mesh);
                }
            }
        }
    }
}