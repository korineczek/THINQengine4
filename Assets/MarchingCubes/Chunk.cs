using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour
{


    private int size = 4;
    public bool isBorderChunk = true;
    public Transform DebugCube;
    private Vector3 ChunkPos;
    public Vector3 ChunkID = new Vector3(0,0,0);

    private bool[,,] chunkVals = new bool[4,4,4];

    //debug cubes
    private DebugCube[,,] debugCubes = new DebugCube[4,4,4];


    private GridCell currentCell;
    private MarchingCubes marchingRenderer;

    private int xPos;
    private int yPos;
    private int zPos;

    //Chunk references
    public int neighborCase = 0;
    public Chunk chunkX;
    public Chunk chunkY;
    public Chunk chunkZ;
    public Chunk chunkXY;
    public Chunk chunkXZ;
    public Chunk chunkYZ;
    public Chunk chunkXYZ;
    public Chunk[] chunkNeighbors;

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
                for (int z = 0; z < size; z++)
                {
                    Transform currentDebug = Instantiate(DebugCube, new Vector3(x + transform.position.x, y + transform.position.y, z + transform.position.z), Quaternion.identity) as Transform;
                    currentDebug.SetParent(this.transform);
                    debugCubes[x, y, z] = currentDebug.GetComponent<DebugCube>();
                    if (Random.Range(0,2) == 0)
                    {
                        chunkVals[x, y, z] = true;
                    }
                    
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
                chunkVals[(int)hitInfo.transform.position.x - xPos,(int)hitInfo.transform.position.y - yPos,(int)hitInfo.transform.position.z - zPos] = true;
                MarchChunk();
            }
        }
	    if (Input.GetKeyUp(KeyCode.U))
	    {
	        MarchChunk();
	    }
	}

    /// <summary>
    /// try to march the entire chunk
    /// </summary>
    public void MarchChunk()
    {
        marchingRenderer.ClearLists();
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.y;
        zPos = (int)transform.position.z;

        //determine if there is a neighboring chunk to stitch with
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for(int z = 0; z < size; z++)
                {
                    int edgeCase = GetEdgeCaseIndex(x, y, z, size);
                    debugCubes[x, y, z].edgeCase = edgeCase;
                    //if we are not bordering any edges yet, proceed normally by adding vertex values
                    if (edgeCase == 0)
                    {
                        currentCell.val[0] = chunkVals[x, y, z];
                        currentCell.val[1] = chunkVals[x, y, z + 1];
                        currentCell.val[2] = chunkVals[x + 1, y, z + 1];
                        currentCell.val[3] = chunkVals[x + 1, y, z];
                        currentCell.val[4] = chunkVals[x, y + 1, z];
                        currentCell.val[5] = chunkVals[x, y + 1, z + 1];
                        currentCell.val[6] = chunkVals[x + 1, y + 1, z + 1];
                        currentCell.val[7] = chunkVals[x + 1, y + 1, z];
                    }
                    //when we arrive at any edge, we have to start checking for available neighboring chunks
                    else
                    {
                        //vertex value 0 never has to be doublechecked, it is always within the current chunk
                        currentCell.val[0] = chunkVals[x, y, z];
                        //other values have to be checked if they are within another chunk or not
                        currentCell.val[1] = LookupTables.NeighborTable[edgeCase, 4] ? chunkVals[x, y, z + 1]         : GetValue(edgeCase,4,x,y,z);
                        currentCell.val[2] = LookupTables.NeighborTable[edgeCase, 5] ? chunkVals[x + 1, y, z + 1] : GetValue(edgeCase, 5, x, y, z);
                        currentCell.val[3] = LookupTables.NeighborTable[edgeCase, 1] ? chunkVals[x + 1, y, z] : GetValue(edgeCase, 1, x, y, z);
                        currentCell.val[4] = LookupTables.NeighborTable[edgeCase, 2] ? chunkVals[x, y + 1, z] : GetValue(edgeCase, 2, x, y, z);
                        currentCell.val[5] = LookupTables.NeighborTable[edgeCase, 6] ? chunkVals[x, y + 1, z + 1] : GetValue(edgeCase, 6, x, y, z);
                        currentCell.val[6] = LookupTables.NeighborTable[edgeCase, 7] ? chunkVals[x + 1, y + 1, z + 1] : GetValue(edgeCase, 7, x, y, z);
                        currentCell.val[7] = LookupTables.NeighborTable[edgeCase, 3] ? chunkVals[x + 1, y + 1, z] : GetValue(edgeCase, 3, x, y, z);
                    }

                    //add vertex positions to the current step
                    currentCell.p[0] = new Vector3(x     - xPos, y     - yPos, z     - zPos);
                    currentCell.p[1] = new Vector3(x     - xPos, y     - yPos, z + 1 - zPos);
                    currentCell.p[2] = new Vector3(x + 1 - xPos, y     - yPos, z + 1 - zPos);
                    currentCell.p[3] = new Vector3(x + 1 - xPos, y     - yPos, z     - zPos);
                    currentCell.p[4] = new Vector3(x     - xPos, y + 1 - yPos, z     - zPos);
                    currentCell.p[5] = new Vector3(x     - xPos, y + 1 - yPos, z + 1 - zPos);
                    currentCell.p[6] = new Vector3(x + 1 - xPos, y + 1 - yPos, z + 1 - zPos);
                    currentCell.p[7] = new Vector3(x + 1 - xPos, y + 1 - yPos, z     - zPos);   
        
                    //DebugSection
                    debugCubes[x, y, z].val = currentCell.val;
                    debugCubes[x, y, z].p = currentCell.p;


                    //march current cell
                    marchingRenderer.MarchCell(currentCell);
                }
            }
        }
        marchingRenderer.CreateMesh(this.GetComponent<MeshFilter>().mesh);
    }

    private bool GetValue(int currentEdge, int pos, int x, int y, int z)
    {
        int actionX = (int)LookupTables.ChunkTable[pos, currentEdge].x;
        int actionY = (int)LookupTables.ChunkTable[pos, currentEdge].y;
        int actionZ = (int)LookupTables.ChunkTable[pos, currentEdge].z;

        switch (actionX)
        {
            case 0:
                actionX = 0;
                break;
            case 1:
                actionX = x;
                break;
            case 2:
                actionX = x + 1;
                break;
        }

        switch (actionY)
        {
            case 0:
                actionY = 0;
                break;
            case 1:
                actionY = y;
                break;
            case 2:
                actionY = y + 1;
                break;
        }

        switch (actionZ)
        {
            case 0:
                actionZ = 0;
                break;
            case 1:
                actionZ = z;
                break;
            case 2:
                actionZ = z + 1;
                break;
        }

        //bitwise operations to determine right chunk
        int bitResult = currentEdge & pos;
        if (bitResult == 0){ bitResult = 1;}
        if (chunkNeighbors[bitResult] != null)
        {
            return chunkNeighbors[bitResult].chunkVals[actionX, actionY, actionZ];
        }

        return false;
    }

    public void SetChunkRefs(int nCase, Chunk x, Chunk y, Chunk z, Chunk xy, Chunk xz, Chunk yz, Chunk xyz)
    {
        neighborCase = nCase;
        chunkX = x;
        chunkY = y;
        chunkZ = z;
        chunkXY = xy;
        chunkXZ = xz;
        chunkYZ = yz;
        chunkXYZ = xyz;

        chunkNeighbors = new Chunk[]{null, chunkX, chunkY, chunkXY, chunkZ, chunkXZ, chunkYZ, chunkXYZ};
    }

    private int GetEdgeCaseIndex(int x, int y, int z, int chunkSize)
    {
        bool xEdge = x == chunkSize - 1;
        bool yEdge = y == chunkSize - 1;
        bool zEdge = z == chunkSize - 1;

        //bitmask conditions to get neighbor index
        int caseIndex = 0;
        if (xEdge) { caseIndex |= 1; }
        if (yEdge) { caseIndex |= 2; }
        if (zEdge) { caseIndex |= 4; }

        //case list:
        //0 n/a
        //1 x
        //2 y
        //3 xy
        //4 z
        //5 xz
        //6 yz
        //7 xyz

        return caseIndex;
    }
}
